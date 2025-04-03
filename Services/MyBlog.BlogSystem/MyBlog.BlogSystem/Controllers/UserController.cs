using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Application.Contract.UserApp.Dto;
using MyBlog.BlogSystem.Domain.AppSettings;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.UserSystem.Application.Contract.UserApp;
using AutoMapper;
using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;
using MyBlog.BlogSystem.Domain.Shared;
using MyBlog.BlogSystem.web.filters;
using FileUploadExtension;
using System.Text.RegularExpressions;

namespace MyBlog.BlogSystem.web.Controllers
{
    [ApiController]
    [Route("MyBlog/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UploadSettings _uploadSettings;
        private readonly IUserService _userService;
        private readonly ICurrentClaims _currentClaims;

        public UserController(
            IConfiguration configuration,
            IOptions<UploadSettings> uploadSettings,
            IUserService userService,
            IMapper mapper,
            ICurrentClaims currentClaims
            )
        {
            _uploadSettings = uploadSettings.Value;
            _userService = userService;
            _currentClaims = currentClaims;
        }

        [HttpPost("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserInputDto userDto)
        {
            var userId = _currentClaims.UserId;

            string emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            if (!Regex.IsMatch(userDto.Email, emailPattern))
            {
                return BadRequest(new { Message = "mail format error" });
            }

            if (userDto.HeadImgFile != null)
            {
                try
                {
                    var uploadContext = new UploadContext(userDto.HeadImgFile, userId, _uploadSettings.serviceAddress, overwrite: true);
                    await uploadContext.FileUploadAsync().FileUploadAsync(
                                    async FileInfo =>
                                    {
                                        string fileName = Path.GetFileName(FileInfo);
                                        var updateResult = await _userService.UpdateUserAsync(userId, userDto.UserName, userDto.Email,userDto.Introduction, fileName);
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
                catch (Exception ex)
                {
                    return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
                }
            }
            else
            {
                // 如果没有上传文件，仍然可以更新其他信息
                var updateResult = await _userService.UpdateUserAsync(userId, userDto.UserName,userDto.Email,userDto.Introduction);
                if (!updateResult)
                {
                    return BadRequest(new { Message = "Failed to update user information." });
                }
                return Ok(new { Message = "User information updated successfully without updating head image." });
            }
            return Ok(new { Message = "User information updated successfully." });
        }


        [HttpGet("GetUserInfo")]
        [Authorize]
        public async Task<UserInfoDto> GetUserInfoAsync()
        {
            var userId = _currentClaims.UserId;
            var res = await _userService.GetUserInfoAsync(userId);
            return res;
        }

        [HttpGet("Users")]
        [Authorize]
        [PermissionFilter(superAdminOnly: true)]
        public async Task<UsersDto> GetUsersAsync(int pageIndex, int pageSize, string? userName = null)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                throw new ArgumentException("Invalid page index or size");
            }
            var res = await _userService.GetUsersAsync(pageIndex, pageSize, userName);
            return res;
        }

        [HttpPost("UpdateUserPassword")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPasswordAsync(string password)
        {
            var userId = _currentClaims.UserId;
            if (userId == null) return BadRequest("User ID cannot be null.");

            var result = await _userService.UpdateUserPasswordAsync(userId, password);
            if (result)
            {
                return Ok(new { message = "Password updated successfully.", status = 200 }); // 返回一个对象
            }
            else
            {
                return StatusCode(500, new { message = "An error occurred while updating the password.", status = 500 });
            }
        }

        [HttpPost("AdminUpdateUser")]
        [Authorize]
        [PermissionFilter(superAdminOnly: true)]
        public async Task<IActionResult> AdminUpdateUserAsync([FromForm] AdminUpdateUserDto updateUser, IFormFile? file)
        {
            if (updateUser.RoleId == null || updateUser.RoleId < 0 )
            {
                return BadRequest(new { Message = "权限错误" });
            }

            if (updateUser.UserId == _currentClaims.UserId)
            {
                return BadRequest(new { Message = "不能操作自身数据" });
            }

            if (string.IsNullOrEmpty(updateUser.UserId))
            {
                return BadRequest(new { Message = "UserId is required." });
            }

            if (file != null)
            {
                try
                {
                    var uploadContext = new UploadContext(file, updateUser.UserId, _uploadSettings.serviceAddress, overwrite: true);
                    await uploadContext.FileUploadAsync().FileUploadAsync(
                                    async FileInfo =>
                                    {
                                        string fileName = Path.GetFileName(FileInfo);
                                        _ = await _userService.AdminUpdateUserAsync(updateUser.UserId, updateUser.UserName, updateUser.Email, updateUser.Password, updateUser.RoleId, fileName);
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
                catch (Exception ex)
                {
                    return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
                }
            }
            // 用户信息更新逻辑
            try
            {
                var res = await _userService.AdminUpdateUserAsync(updateUser.UserId, updateUser.UserName, updateUser.Email, updateUser.Password, updateUser.RoleId);
                if (!res)
                {
                    Console.WriteLine($"Update failed for UserId: {updateUser.UserId}, RoleId: {updateUser.RoleId}");
                    return BadRequest(new { Message = "Failed to update user information." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error." });
            }

            return Ok(new { Message = "User information updated successfully without updating head image." });
        }

        [HttpGet("BaneUser")]
        [Authorize]
        [PermissionFilter(superAdminOnly: true)]
        public async Task<IActionResult> BanUserAsnyc(string UserId, string Status)
        {
            if (Status != "0" && Status != "1")
            {
                return BadRequest(new { Message = "Status must be either 0 or 1." });
            }

            int statusInt = int.Parse(Status);

            var res = await _userService.BanUserAsync(UserId, statusInt);
            if (!res)
            {
                return BadRequest(new { Message = "Failed to update user information." });
            }
            return Ok(new { message = "Updated successfully.", status = 200 });
        }


        [HttpGet("DelUser")]
        [Authorize]
        [PermissionFilter(superAdminOnly: true)]
        public async Task<IActionResult> DelUserAsync(string userId)
        {
            return Ok(new { message = "updated successfully.", status = 200 });
        }
    }
}
