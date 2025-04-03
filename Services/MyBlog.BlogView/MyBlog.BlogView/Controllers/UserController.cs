using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.BlogSystem.Domain.Shared;
using MyBlog.BlogView.Application.Contract.PostApp;
using MyBlog.UserSystem.Application.Contract.UserApp;
using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;

namespace MyBlog.BlogView.web.Controllers
{
    [Route("MyBlogView/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService {  get; set; }
        private IPostService _postService { get; set; }
        private readonly ICurrentClaims _currentClaims;

        public UserController(
            IUserService userService,
            IPostService postService,
            ICurrentClaims currentClaims) 
        {
            this._userService = userService;
            this._postService = postService;
            this._currentClaims = currentClaims;
        }

        [HttpGet("GetActiveAdminUser")]
        public async Task<UsersDto> GetActiveAdminUsersAsync(int pageIndex, int pageSize)
        {
            var userInfoDtos = await _userService.GetActiveAdminUsersAsync(pageIndex, pageSize);
            foreach (var userInfo in userInfoDtos.userInfoDtos) {
                userInfo.BlogCount = await _postService.GetPostCountByUserIdAsync(userInfo.UserId);
            }
            return userInfoDtos;
        }

        [HttpGet("GetUserInfo")]
        [Authorize]
        public async Task<UserInfoDto> GetUserInfoAsync()
        {
            var userId = _currentClaims.UserId;
            var res = await _userService.GetUserInfoAsync(userId);
            return res;
        }
    }
}
