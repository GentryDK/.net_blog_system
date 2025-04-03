using Microsoft.Extensions.Hosting;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Domain.Managers;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using MyBlog.BlogSystem.Domain.Shared.Redis.Dtos;
using MyBlog.BlogView.Application.Contract.PostApp;
using MyBlog.BlogView.Application.Contract.PostApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.BlogView.Application.PostApp
{
    public class PostService : ApplicationService, IPostService
    {
        private PostManager _postManager {  get; set; }
        private PostTypeManager _postTypeManager { get; set; }

        public PostService(PostManager postManager,PostTypeManager postTypeManager) 
        {
            this._postManager = postManager;
            this._postTypeManager = postTypeManager;
        }

        /// <summary>
        /// 获取到帖子
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<PostInfoDto> GetPostsDtoAsync(PostFilter filters)
        {
            PostInfoDto postInfoDto = new PostInfoDto();
            var skip = (filters.pageIndex - 1) * filters.pageSize;

            //// 获取分页数据和总数
            var (posts, totalCount) = await _postManager.GetFilteredPostsWithCountAsync(filters, skip);

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

        public async Task<int> GetPostCountByUserIdAsync(string userId)
        {
            return await _postManager.GetPostByUserIdAsnyc(userId);
        }

        public async Task<PostDetailDto> GetPostDtoAsync(string postId)
        {
            var res = await _postManager.GetPostByIdAsnyc(postId);
            var dto = ObjectMapper.Map<Post, PostDetailDto>(res);
            if (dto.PostTypeId == null) return dto;
            var postType = await _postTypeManager.GetPostTypeById(dto.PostTypeId);
            dto.PostTypeName = postType?.PostTypeName;
            return dto;
        }
    }
}
