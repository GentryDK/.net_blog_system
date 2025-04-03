using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application.Contract.PostApp
{
    public interface IRecyclePostService
    {
        Task<PostInfoDto> GetRecyclePostsAsync(int pageIndex, int pageSize, string? postTitle);

        Task<bool> HardDeletePostAsync(string postId);

        Task<PostDto> RecoverPostAsync(string postId);
    }
}
