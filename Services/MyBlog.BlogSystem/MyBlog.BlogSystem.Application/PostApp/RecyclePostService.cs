using MyBlog.BlogSystem.Application.Contract.PostApp;
using MyBlog.BlogSystem.Domain.Managers;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared.Redis;
using MyBlog.BlogSystem.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MyBlog.BlogSystem.Application.PostApp
{
    public class RecyclePostService: ApplicationService, IRecyclePostService
    {
        private readonly PostManager PostManager;
        private readonly PostTypeManager _postTypeManager;
        private readonly ICurrentClaims _currentClaims;

        private IDistributedCache<PostDraft> _postDraftDistributedCache { get; set; }

        public RecyclePostService(
            IRepository<Post> postRepo,
            PostManager postManager,
            PostTypeManager postTypeManager,
            IDistributedCache<PostDraft> postDraftDistributedCache,
            ICurrentClaims currentClaim
            )
        {
            this._currentClaims = currentClaim;
            this.PostManager = postManager;
            this._postTypeManager = postTypeManager;
            this._postDraftDistributedCache = postDraftDistributedCache;
        }

        /// <summary>
        /// 获取回收站中的帖子
        /// </summary>
        /// <returns></returns>
        public async Task<PostInfoDto> GetRecyclePostsAsync(int pageIndex, int pageSize, string? postTitle)
        {
            PostInfoDto postInfoDto = new PostInfoDto();
            var skip = (pageIndex - 1) * pageSize;

            var userId = _currentClaims.UserId;
            bool isSuperAdmin = _currentClaims.IsSuperAdmin;

            if (userId == null)
            {
                throw new InvalidOperationException("User ID cannot be null.");
            }

            var (posts, totalCount) = await PostManager.GetRecyclePostsAsync(skip, pageSize, postTitle, isSuperAdmin ? null : userId);

            postInfoDto.PostCount = totalCount;  // 获取总数
            var dtos = ObjectMapper.Map<List<Post>, List<PostDto>>(posts);
            foreach (var post in dtos)
            {
                if (post.PostTypeId == null) continue;
                var postType = await _postTypeManager.GetPostTypeById(post.PostTypeId);
                post.PostTypeName = postType?.PostTypeName;
            }
            postInfoDto.PostListDto = dtos;
            return postInfoDto;
        }

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="postTypeId"></param>
        /// <returns></returns>
        public async Task<bool> HardDeletePostAsync(string postId)
        {
            return await PostManager.HardDeletePostAsync(postId);
        }

        /// <summary>
        /// 恢复删除的Post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task<PostDto> RecoverPostAsync(string postId)
        {
            var post = await PostManager.RecoverSoftPostAsync(postId);
            var postDto = ObjectMapper.Map<Post, PostDto>(post);
            return postDto;
        }
    }
}
