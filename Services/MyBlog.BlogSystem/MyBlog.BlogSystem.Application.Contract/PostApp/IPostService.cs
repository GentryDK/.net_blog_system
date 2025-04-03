using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using MyBlog.BlogSystem.Domain.Managers;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using MyBlog.BlogSystem.Domain.Shared.Redis.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.BlogSystem.Application.Contract.PostApp
{
    public interface IPostService:IApplicationService
    {
        Task<PostInfoDto> GetPostDtoAsync(PostFilter filters);

        Task<PostServiceResult> UpdatePostAsync(PostCreateDto createDto);

        Task<bool> UpdatePostCoverAsync(string cover);

        Task<PostDaftDto> AutoSave(PostDaftDto draftDto);

        Task<bool> RemovePostAsync(string postId);
    }
}
