using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.UserSystem.web.Extensions;
using MyBlog.UserSystem.Application.Contract.UserApp;
using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;
using MyBlog.UserSystem.Application.UserApp;
using MyBlog.UserSystem.Domain.AppSettings;
using System.Security.Claims;
using UserSystem.Application.Contract.UserApp.Dtos;

namespace MyBlog.UserSystem.Controllers
{
    [ApiController]
    [Route("user/[controller]")]
    public class UserController : ControllerBase
    {
        public IUserService UserService { get; set; }
        public readonly UploadSettings UploadSettings;

        //IOptions<T> 是 ASP.NET Core 中的一部分，用于注入强类型配置对象。通过它，配置数据可以从配置文件（如 appsettings.json）或其他配置源注入到应用程序中。
        //强类型配置： 使用 IOptions<T> 可以确保配置的类型安全性，避免了硬编码和类型转换的麻烦
        //延迟加载： 配置在需要的时候才会被加载，并不会在应用程序启动时一次性加载所有配置数据，提高了性能。
        //默认值和重新加载： IOptions<T> 支持设置默认值，并且当配置文件发生变化时，自动重新加载配置
        public UserController(
            IUserService userService,
            IOptions<UploadSettings> uploadSettings
            )
        {
            this.UserService = userService;
            this.UploadSettings = uploadSettings.Value;
        }

        [HttpPost]
        public async Task<bool> RegistUserAsync(UserCreateDto createInput)
        {
            return await UserService.RegistUserAsync(createInput);
        }

        [HttpPost("SendEmailCode")]
        public async Task<bool> SendEmailCode(EMailDto emailDto)
        {
            return await UserService.SendEmailCode(emailDto);
        }



        [HttpGet]
        public async Task<string> CheckLogin(string email, string password)
        {
            var token = await UserService.CheckLoginAsync(email, password);
            return token;
        }

        [HttpGet("RefreshToken")]
        [Authorize]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var res = await UserService.RefreshToken();
            if (!res.isSuccess)
            {
                return Unauthorized(res.token);
            }
            return res.token;
        }

        private async Task<bool> SaveUserAvatarAsync(string userId, IFormFile headImgFile)
        {
            var res = await headImgFile.UploadAsync(userId, UploadSettings.UploadUrl,async (fileName) =>
                    {
                    return await UserService.UpdateUserHeadUrl($"IconFile/{fileName}");
                    }
                );
            return res != null;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            var uniqueFileName = Guid.NewGuid().ToString();
            var res = await file.UploadAsync(uniqueFileName, UploadSettings.UploadUrl, async (n) =>
            {
                await UserService.UpdateUserHeadUrl($"IconFile/{n}");
                return Ok(new { FilePath = $"IconFile/{n}" });
            });
            return Ok(new { FilePath = $"IconFile/{res}" });
        }
    }
}
