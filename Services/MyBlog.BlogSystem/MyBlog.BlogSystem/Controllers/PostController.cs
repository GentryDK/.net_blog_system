using FileUploadExtension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.BlogSystem.Application.Contract.CommonDto;
using MyBlog.BlogSystem.Application.Contract.PostApp;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using MyBlog.BlogSystem.Application.PostApp;
using MyBlog.BlogSystem.Application.PostTypeApp;
using MyBlog.BlogSystem.Domain.AppSettings;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using MyBlog.BlogSystem.Domain.Shared.Redis.Dtos;
using MyBlog.BlogSystem.web.filters;
using System.IO;

namespace MyBlog.BlogSystem.web.Controllers
{
    [ApiController]
    [Route("MyBlog/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService PostService;
        public readonly UploadSettings UploadSettings;



        public PostController(
            IPostService postService,
            IOptions<UploadSettings> uploadSettings
            )
        {
            this.PostService = postService;
            this.UploadSettings = uploadSettings.Value;
        }

        //对于复杂对象参数（如类对象），如果不加[FromQuery]，
        //ASP.NET Core 会尝试从请求体中解析参数，这在 GET 请求中通常是不允许的。
        //因此，对于复杂对象参数，加上[FromQuery] 是必要的
        [HttpGet("GetPosts")]
        [Authorize]
        public async Task<PostInfoDto> GetPostDtosAsync([FromQuery] PostFilter filter)
        {
            return await PostService.GetPostDtoAsync(filter);                     
        }

        [HttpPost("AddPost")]
        [Authorize]
        [PermissionFilter]
        public async Task<IActionResult> AddPostAsync(PostCreateDto createDto)
        {
            var result = await PostService.UpdatePostAsync(createDto);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            if (createDto.CoverFile != null)
            {
                var uploadContext = new UploadContext(createDto.CoverFile, result.Post.Id, UploadSettings.serviceAddress, overwrite: true);
                await uploadContext.FileUploadAsync().FileUploadAsync(
                        async FileInfo =>
                        {
                                await PostService.UpdatePostCoverAsync(FileInfo);
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
            return Ok(result.Message);
        }

        //TODO 添加权限的验证
        [HttpGet("RemovePost")]
        [Authorize]
        [PermissionFilter]
        public async Task<bool> RemovePostAsync(string postId)
        {
            return await PostService.RemovePostAsync(postId);
        }

        [HttpPost("AutoSave")]
        [Authorize]
        [PermissionFilter]
        public async Task<PostDaftDto> AutoSaveAsync([FromForm]PostDaftDto daftDto)
        {
            var result = await PostService.AutoSave(daftDto);
            return result;
        }

        [HttpPost("Picture")]
        [Authorize]
        [PermissionFilter]
        public async Task<string> AddPictureAsync(IFormFile pictureFile)
        {
            var uniqueFileName = Guid.NewGuid().ToString();

            var uploadContext = new UploadContext(pictureFile, uniqueFileName, UploadSettings.serviceAddress, overwrite: true);
            var path = await uploadContext.FileUploadAsync().FileUploadAsync(
                        async FileInfo =>{},
                                FileInfo =>
                                {
                                    throw new Exception("当前文件已经存在");
                                },
                                ex =>
                                {
                                    throw ex;
                                });
            string fileName = Path.GetFileName(path);
            return fileName;
        }
    }
}
