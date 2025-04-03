using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using MyBlog.BlogSystem.Domain.Shared.Redis.Dtos;
using MyBlog.BlogView.Application.Contract.PostApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.BlogView.Application.Contract.PostApp
{
    public interface IPostService:IApplicationService
    {
        Task<PostInfoDto> GetPostsDtoAsync(PostFilter filters);

        Task<PostDetailDto> GetPostDtoAsync(string postId);

        Task<int> GetPostCountByUserIdAsync(string userId);
    }
}
