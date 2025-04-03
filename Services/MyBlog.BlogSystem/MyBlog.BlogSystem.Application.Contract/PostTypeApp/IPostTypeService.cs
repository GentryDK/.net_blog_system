using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.BlogSystem.Application.Contract.PostTypeApp
{
    public interface IPostTypeService : IApplicationService
    {
         Task<List<PostTypeDto>> GetPostTypeAsync(int pageIndex = 1, int pageSize = 5);

        Task<PostTypeServiceResult> UpdatePostTypeAsync(PostTypeDto postTypeDto);

        Task<bool> UpdatePostTypeCoverAsync(string cover);

        Task<bool> UpdatePostTpyeOrderAsync(string postTypeId, int order);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="postTypeId"></param>
        /// <returns></returns>
        Task<bool> RemovePostTypeAsync(string postTypeId);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="postTypeId"></param>
        /// <returns></returns>
        Task<bool> HardDeletePostTypeAsync(string postTypeId);
    }
}
