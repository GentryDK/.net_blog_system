using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using MyBlog.BlogView.Application.Contract.PostApp;
using MyBlog.BlogView.Application.Contract.PostApp.Dto;
using MyBlog.BlogView.Application.PostApp;

namespace MyBlog.BlogView.web.Controllers
{
    [Route("MyBlogView/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private IPostService _postService {  get; set; }

        public PostController(IPostService postService)
        {
            this._postService = postService;
        }

        [HttpGet("GetPosts")]
        public async Task<PostInfoDto> GetPostInfoAsync([FromQuery] PostFilter filter)
        {
            return await _postService.GetPostsDtoAsync(filter);
        }

        [HttpGet("GetPost")]
        public async Task<PostDetailDto> GetPostInfoAsync(string postId)
        {
            return await _postService.GetPostDtoAsync(postId);
        }
    }
}
