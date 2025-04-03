using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.BlogSystem.Application.Contract.PostApp;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Application.PostApp;
using MyBlog.BlogSystem.Domain.AppSettings;
using MyBlog.BlogSystem.web.filters;

namespace MyBlog.BlogSystem.web.Controllers
{
    [Route("MyBlog/[controller]")]
    [ApiController]
    public class RecyclePostController : ControllerBase
    {
        private readonly IRecyclePostService _recycelePostService;
        public readonly UploadSettings UploadSettings;

        public RecyclePostController(
            IRecyclePostService recycelePostService,
            IOptions<UploadSettings> uploadSettings
            )
        {
            this._recycelePostService = recycelePostService;
            this.UploadSettings = uploadSettings.Value;
        }

        [HttpGet("GetRecycleBinPost")]
        [Authorize]
        [PermissionFilter]
        public async Task<PostInfoDto> GetRecycleBinPostDtosAsync(int pageIndex, int pageSize, string? postTitle)
        {
            return await _recycelePostService.GetRecyclePostsAsync(pageIndex, pageSize, postTitle);
        }

        [HttpGet("PermanentlyDeletePost")]
        [Authorize]
        [PermissionFilter]
        public async Task<IActionResult> PermanentlyDeletePostAsync(string postId)
        {
           var res = await _recycelePostService.HardDeletePostAsync(postId);
           if (res)
            {
                return Ok(new { Message = "successful delete post" });
            }
            return BadRequest(new { Message = "failled to delete post" });
        }

        [HttpGet("RecoverDeletedPost")]
        [Authorize]
        [PermissionFilter]
        public async Task<IActionResult> RecoverDeletedPostAsync(string postId)
        {
            var res = await _recycelePostService.RecoverPostAsync(postId);
            if (res!=null)
            {
                return Ok(new { Message = "successful recover post" });
            }
            return BadRequest(new { Message = "failled to recover post" });
        }
    }
}
