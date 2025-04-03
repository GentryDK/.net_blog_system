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

        Task<int> GetPostTypeCountAsync();

        Task<PostTypeDto> GetPostTypeByIdAsync(string postTypeId);
    }
}
