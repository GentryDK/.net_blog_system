using MyBlog.BlogSystem.Application.Contract.PostTypeApp;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using MyBlog.BlogSystem.Domain.Managers;
using MyBlog.BlogSystem.Domain.PostInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;

namespace MyBlog.BlogView.Application.PostTypeApp
{
    public class PostTypeService : ApplicationService, IPostTypeService
    {
        public readonly PostTypeManager _postTypeManager;
        public readonly PostManager _postManager;

        public PostTypeService(
            PostTypeManager postTypeManager,
            PostManager postManager
            )
        {
            this._postTypeManager = postTypeManager;
            this._postManager = postManager;
        }

        public async Task<List<PostTypeDto>> GetPostTypeAsync(int pageIndex = 1, int pageSize = 5)
        {
            var skip = (pageIndex - 1) * pageSize;
            var res = await _postTypeManager.GetPostTypeAsync(skip, pageSize);
            var postTypeDtos = new List<PostTypeDto>();
            foreach (var item in res)
            {

                var postTypeDto = ObjectMapper.Map<PostType, PostTypeDto>(item);

                int count = await _postManager.GetPostCountByTypeAsymc(item.Id);

                postTypeDto.Count = count;
                postTypeDtos.Add(postTypeDto);
            }
            // 按照 Order 属性升序排序
            var sortedPostTypeDtos = postTypeDtos.OrderBy(dto => dto.Order).ToList();
            return sortedPostTypeDtos;
        }

        public async Task<PostTypeDto> GetPostTypeByIdAsync(string postTypeId)
        {
           var postType = await _postTypeManager.GetPostTypeById(postTypeId);
            int count = await _postManager.GetPostCountByTypeAsymc(postTypeId);
            var postTypeDto = ObjectMapper.Map<PostType, PostTypeDto>(postType);
            postTypeDto.Count = count;
            return postTypeDto;
        }

        /// <summary>
        /// 得到当前的PostType的数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetPostTypeCountAsync()
        {
            return await _postTypeManager.GetPostTypeCountAsync();
        }
    }
}
