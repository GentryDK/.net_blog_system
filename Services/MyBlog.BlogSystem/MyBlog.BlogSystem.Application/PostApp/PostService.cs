using AutoMapper.Internal.Mappers;
using MyBlog.BlogSystem.Domain.Managers;
using MyBlog.BlogSystem.Domain.PostInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using Volo.Abp.Application.Services;
using MyBlog.BlogSystem.Application.Contract.PostApp;
using Microsoft.AspNetCore.Http.HttpResults;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using Microsoft.EntityFrameworkCore;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using AngleSharp.Html;
using Volo.Abp.Caching;
using MyBlog.BlogSystem.Domain.Shared.Redis;
using Volo.Abp.Users;
using MyBlog.BlogSystem.Domain.Shared.Redis.Dtos;
using Microsoft.AspNetCore.Http;
using MyBlog.BlogSystem.Domain.Shared;
using Microsoft.Extensions.Caching.Distributed;
using MyBlog.BlogSystem.Application.Extension;
using MyBlog.BlogSystem.Application.SensitiveApp;

namespace MyBlog.BlogSystem.Application.PostApp
{
    public class PostService: ApplicationService, IPostService
    {
        private readonly PostManager PostManager;
        private readonly PostTypeManager _postTypeManager;
        private readonly SensitiveService _sensitiveService;
        private readonly ICurrentClaims _currentClaims;

        private IDistributedCache<PostDraft> _postDraftDistributedCache { get; set; }

        public PostService(
            IRepository<Post> postRepo,
            PostManager postManager,
            SensitiveService sensitiveService,
            PostTypeManager postTypeManager,
            IDistributedCache<PostDraft> postDraftDistributedCache,
            ICurrentClaims currentClaim
            )
        {
            this._currentClaims = currentClaim;
            this.PostManager = postManager;
            this._sensitiveService = sensitiveService;
            this._postTypeManager = postTypeManager;
            this._postDraftDistributedCache = postDraftDistributedCache;
        }

        public async Task<PostDto> GetPostDetail(string postId)
        {
            var post = await PostManager.GetPostByIdAsnyc(postId);
           var res = ObjectMapper.Map<Post, PostDto>( post );
            return res;
        }

        /// <summary>
        /// 获取到帖子
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<PostInfoDto> GetPostDtoAsync(PostFilter filters)
        {
            PostInfoDto postInfoDto = new PostInfoDto();
            var skip = (filters.pageIndex - 1) * filters.pageSize;

            var userId = _currentClaims.UserId;
            bool isSuperAdmin = _currentClaims.IsSuperAdmin;

            if (userId == null)
            {
                throw new InvalidOperationException("User ID cannot be null.");
            }

            // 获取分页数据和总数
            var (posts, totalCount) = await PostManager.GetFilteredPostsWithCountAsync(filters, skip, isSuperAdmin ? null : userId);

            postInfoDto.PostCount = totalCount;  // 获取总数
            var dtos = ObjectMapper.Map<List<Post>, List<PostDto>>(posts);
            foreach (var post in dtos)
            {
                if (post.PostTypeId == null) continue;
                var postType = await _postTypeManager.GetPostTypeById(post.PostTypeId);
                post.PostTypeName = postType?.PostTypeName;
            }
            postInfoDto.PostListDto = dtos;

            postInfoDto = await RetrieveDraftsFromCache(postInfoDto, filters, userId);
            return postInfoDto;
        }

        /// <summary>
        /// 拿到用户redis中的零时创建的帖子草稿
        /// </summary>
        /// <param name="postInfoDto"></param>
        /// <param name="filters"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<PostInfoDto> RetrieveDraftsFromCache(PostInfoDto postInfoDto,PostFilter filters, string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var draft = await _postDraftDistributedCache.GetAsync(userId);
                if (draft != null)
                {
                    draft = BlogExtension.EnsureInitialized(draft);
                    // 手动应用筛选条件
                    if (ApplyFilters(draft, filters))
                    {
                        var postDto = ObjectMapper.Map<PostDraft, PostDto>(draft);
                        postInfoDto.PostListDto.Add(postDto);
                    }
                }
            }
            return postInfoDto;
        }

        private bool ApplyFilters(PostDraft draft, PostFilter filters)
        {
            return (string.IsNullOrEmpty(filters.PostTitle) || draft.PostTitle.Contains(filters.PostTitle)) &&
                   (!filters.State.HasValue || draft.State == filters.State) &&
                   (string.IsNullOrEmpty(filters.PostTypeId) || draft.PostTypeId == filters.PostTypeId);
        }


        /// <summary>
        /// 添加或更新帖子
        /// </summary>
        /// <param name="createDto"></param>
        /// <returns></returns>
        public async Task<PostServiceResult> UpdatePostAsync(PostCreateDto createDto)
        {
            var userId = _currentClaims.UserId;
            var userName = _currentClaims.UserName;

            await _postDraftDistributedCache.RemoveAsync(userId);

            if (createDto.Id == null)
            {
                return await AddNewPostAsync(createDto,userId,userName);
            }
            return await UpdateExistingPostAsync(createDto);
        }

        private async Task<PostServiceResult> AddNewPostAsync(PostCreateDto createDto,string userId,string userName)
        {
            var temp = ObjectMapper.Map<PostCreateDto, Post>(createDto);

            //获取到敏感词
            var words = await _sensitiveService.GetWordsInCacheAsync();
            //过滤帖子的敏感词
            temp.FilterSensitiveWords(words);

            var post = await PostManager.AddPostAsync(temp, userId, userName);

            return new PostServiceResult
            {
                Message = "发帖成功",
                Success = true,
                Post = post
            };
        }

        private async Task<PostServiceResult> UpdateExistingPostAsync(PostCreateDto createDto)
        {
            var post = await PostManager.GetPostByIdAsnyc(createDto.Id);
            //获取到敏感词
            var words = await _sensitiveService.GetWordsInCacheAsync();

            if (post == null)
            {
                return new PostServiceResult
                {
                    Success = false,
                    Message = "帖子id错误或者为null"
                };
            }

            post.PostTitle = createDto.PostTitle;
            post.PostTypeId = createDto.PostTypeId;
            post.PostContent = createDto.PostContent;
            //过滤帖子的敏感词
            post.FilterSensitiveWords(words);
            post.EditDate = DateTime.Now;

            var result = await PostManager.UpdatePostAsync(post);
            return new PostServiceResult
            {
                Message = "帖子更新成功",
                Success = true,
                Post = result
            };
        }

        public async Task<PostDaftDto> AutoSave(PostDaftDto draftDto)
        {
            var userId = _currentClaims.UserId;
            // 使用一个方法来确保所有属性已初始化
            var draft = BlogExtension.EnsureInitialized(ObjectMapper.Map<PostDaftDto, PostDraft>(draftDto));

            //配置过期时间
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
            };
            await _postDraftDistributedCache.SetAsync(userId, draft, cacheOptions);
            return draftDto;
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task<bool> RemovePostAsync(string postId)
        {
         return  await PostManager.SoftPostAsync(postId);
        }


        /// <summary>
        /// 更新帖子封面
        /// </summary>
        /// <param name="cover"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePostCoverAsync(string cover)
        {
            string fileName = Path.GetFileName(cover);
            return await PostManager.UpdatePostCoverUrl(fileName);
        }
    }
}
