using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
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
using Microsoft.Extensions.Configuration;

namespace MyBlog.BlogSystem.Application.PostTypeApp
{
    public class PostTypeService : ApplicationService, IPostTypeService
    {
        public readonly PostTypeManager PostTypeManager;
        public readonly PostManager PostManager;
        private readonly IConfiguration _configuration;

        public PostTypeService(
            PostTypeManager postTypeManager,
            PostManager postManager,

            IConfiguration configuration
            ) {
            this.PostTypeManager = postTypeManager;
            this.PostManager = postManager;
            this._configuration =configuration;
        }

        public async Task<List<PostTypeDto>> GetPostTypeAsync(int pageIndex = 1, int pageSize = 5)
        {
            var skip = (pageIndex - 1) * pageSize;
            var res = await PostTypeManager.GetPostTypeAsync(skip, pageSize);
            var postTypeDtos = new List<PostTypeDto>();
            foreach (var item in res) {

                var postTypeDto = ObjectMapper.Map<PostType, PostTypeDto>(item);

                int count = await PostManager.GetPostCountByTypeAsymc(item.Id);

                postTypeDto.Count = count;
                postTypeDtos.Add(postTypeDto);
            }
            // 按照 Order 属性升序排序
            var sortedPostTypeDtos = postTypeDtos.OrderBy(dto => dto.Order).ToList();
            return sortedPostTypeDtos;
        }

        /// <summary>
        /// 设置新的PostType到数据库中
        /// </summary>
        /// <param name="postTypeDto"></param>
        /// <returns></returns>
        public async Task<PostTypeDto> AddPostTypeAsync(PostTypeDto postTypeDto)
        {
            var postType = ObjectMapper.Map<PostTypeDto, PostType>(postTypeDto);
            var addPostType = await PostTypeManager.SetPostTypeAsync(postType);
            return ObjectMapper.Map<PostType, PostTypeDto>(addPostType);
        }

        /// <summary>
        /// 更新PostType,存在就更新，不存在就添加新的
        /// </summary>
        /// <param name="postTypeDto"></param>
        /// <returns></returns>
        public async Task<PostTypeServiceResult> UpdatePostTypeAsync(PostTypeDto postTypeDto)
        {
            //判断是否达到最大帖子类型的上限
            var postTypeCount = await PostTypeManager.GetPostTypeCountAsync();
            if (postTypeCount >= _configuration.GetValue<int>("PostTypeCount"))
            {
                return new PostTypeServiceResult
                {
                    Success = false,
                    Message = "帖子类型超过最大数量上限"
                };
            }

            var updatePostType = await PostTypeManager.GetPostTypeById(postTypeDto.Id);
            //通过名称取出来的
            if (updatePostType != null)
            {
                //当名称被更改的时候,查看更改的名称是否重复
                if (updatePostType.PostTypeName != postTypeDto.PostTypeName)
                {
                    var postTypeName = await PostTypeManager.GetPostTypeByTypeNameAsync(postTypeDto.PostTypeName);
                    if (postTypeName != null)
                    {
                        return new PostTypeServiceResult
                        {
                            Success = false,
                            Message = "当前类型名称已存在列表或回收站中"
                        };
                    }
                }

                updatePostType.PostTypeName = postTypeDto.PostTypeName;
                updatePostType.TypeBrief = postTypeDto.TypeBrief;
                var res = await PostTypeManager.UpdatePostTypeAsync(updatePostType);
                return new PostTypeServiceResult
                {
                    Success = true,
                    PostType = updatePostType
                };
            }
            else //add
            {                
                //帖子名称重复
                var postTypeName = await PostTypeManager.GetPostTypeByTypeNameAsync(postTypeDto.PostTypeName);
                if (postTypeName != null)
                {
                    return new PostTypeServiceResult
                    {
                        Success = false,
                        Message = "当前类型名称已存在列表或回收站中"
                    };
                }
                var res = await AddPostTypeAsync(postTypeDto);
                var addPostType = ObjectMapper.Map<PostTypeDto, PostType>(res);

                return new PostTypeServiceResult
                {
                    Success = true,
                    PostType = addPostType
                };
            }
        }

        /// <summary>
        /// 更新帖子封面
        /// </summary>
        /// <param name="cover"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePostTypeCoverAsync(string cover)
        {
            string fileName = Path.GetFileName(cover);
            return await PostTypeManager.UpdatePostTypeCoverUrl(fileName);
        }

        /// <summary>
        /// 更新PostType的顺序
        /// </summary>
        /// <param name="postTypeId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePostTpyeOrderAsync(string postTypeId,int order)
        {
            var postType = await PostTypeManager.GetPostTypeById(postTypeId);
            postType.Order = order;
            await PostTypeManager.UpdatePostTypeAsync(postType);
            return true;
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="postTypeId"></param>
        /// <returns></returns>
        public async Task<bool> RemovePostTypeAsync(string postTypeId)
        {
            await PostTypeManager.SoftPostTypeAsync(postTypeId);
            await PostManager.SoftPostTypeByTypeIdAsync(postTypeId);
            return true;
        }

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="postTypeId"></param>
        /// <returns></returns>
        public async Task<bool> HardDeletePostTypeAsync(string postTypeId)
        {
            await PostTypeManager.HardDeletePostTypeAsync(postTypeId);
            await PostManager.HardDeletePostByTpyeIdAsync(postTypeId);
            return true;
        }
    }
}
