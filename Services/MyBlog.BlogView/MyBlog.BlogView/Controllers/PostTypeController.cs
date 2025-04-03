using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using MyBlog.BlogSystem.Domain.AppSettings;

namespace MyBlog.BlogView.web.Controllers
{
    [Route("MyBlogView/[controller]")]
    [ApiController]
    public class PostTypeController : ControllerBase
    {
        private readonly IPostTypeService _postTypeService;

        public PostTypeController(
            IPostTypeService postTypeService,
            IOptions<UploadSettings> uploadSettings)
        {
            this._postTypeService = postTypeService;
        }

        [HttpGet("GetPostTypes")]
        public async Task<List<PostTypeDto>> GetPostTypesAsync(int pageIndex, int pageSize)
        {
            return await _postTypeService.GetPostTypeAsync(pageIndex, pageSize);
        }

        [HttpGet("GetPostType")]
        public async Task<PostTypeDto> GetPostTypesAsync(string postTypeId)
        {
            return await _postTypeService.GetPostTypeByIdAsync(postTypeId);
        }

        [HttpGet("GetPostTypeCount")]
        public async Task<int> GetPostTypeCountAsync()
        {
            return await _postTypeService.GetPostTypeCountAsync();
        }

    }
}
