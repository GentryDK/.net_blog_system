using FileUploadExtension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.BlogSystem.Application.Contract.PostApp;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using MyBlog.BlogSystem.Domain.AppSettings;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.web.filters;

namespace MyBlog.BlogSystem.web.Controllers
{
    [ApiController]
    [Route("MyBlog/[controller]")]
    public class PostTypeController : ControllerBase
    {
        private readonly IPostTypeService PostTypeService;
        public readonly UploadSettings UploadSettings;

      public PostTypeController(
          IPostTypeService postTypeService,
          IOptions<UploadSettings> uploadSettings) 
        {
            this.PostTypeService = postTypeService;
            this.UploadSettings = uploadSettings.Value;
        }

        [HttpGet("GetPostTypes")]
        [Authorize]
        public async Task<List<PostTypeDto>> GetPostTypesAsync(int pageIndex, int pageSize)
        {
           return await PostTypeService.GetPostTypeAsync(pageIndex, pageSize);
        }

        [HttpPost("AddPostType")]
        [Authorize]
        [PermissionFilter(superAdminOnly:true)]
        public async Task<IActionResult> UploadPostTypeAsync(AddPostTypeDto addPostTypeDto)
        {
           var postTpye = new PostTypeDto(addPostTypeDto.PostTypeId,addPostTypeDto.PostTypeName,addPostTypeDto.TypeBrief);
           var result = await PostTypeService.UpdatePostTypeAsync(postTpye);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            if (addPostTypeDto.CoverFile!=null)
            {
                var uploadContext = new UploadContext(addPostTypeDto.CoverFile, result.PostType.Id, UploadSettings.serviceAddress, overwrite: true);
                await uploadContext.FileUploadAsync().FileUploadAsync(
                                async FileInfo =>
                                {
                                    await PostTypeService.UpdatePostTypeCoverAsync(FileInfo);
                                },
                                FileInfo =>
                                {
                                    throw new Exception("当前文件已经存在");
                                },
                                ex =>
                                {
                                    throw ex;
                                });
            }
            return Ok();
        }

        [HttpPost("UpdatePostTypeOrder")]
        [Authorize]
        [PermissionFilter(superAdminOnly: true)]
        public async Task<IActionResult> UpdatePostTypeOrderAsync(List<PostTypeOrderDto> postTypeOrders)
        {
            if (postTypeOrders == null) return BadRequest("Invalid request.");
            foreach (var postTypeOrder in postTypeOrders)
            {
                await PostTypeService.UpdatePostTpyeOrderAsync(postTypeOrder.Id, postTypeOrder.Order);
            }
            return Ok(true);
        }

        [HttpGet("RemovePostTpye")]
        [Authorize]
        [PermissionFilter(superAdminOnly: true)]
        public async Task<bool> RemovePostTypeAsync(string postTypeId)
        {
            await PostTypeService.RemovePostTypeAsync(postTypeId);
            return true;
        }
    }
}
