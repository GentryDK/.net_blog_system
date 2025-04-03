using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;
using MyBlog.UserSystem.Domain.Account;
using MyBlog.UserSystem.Domain.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserSystem.Application.Contract.UserApp.Dtos;
using Volo.Abp.Application.Services;

namespace MyBlog.UserSystem.Application.Contract.UserApp
{
    public interface IUserService : IApplicationService
    {
        Task<string> CheckLoginAsync(string email, string passward);

        Task<bool> SendEmailCode(EMailDto emailDto);

        Task<bool> RegistUserAsync(UserCreateDto createInput);

        Task<(string token, bool isSuccess)> RefreshToken();

        Task<bool> UpdateUserAsync(string userId, string? userName = null, string? email = null, string introduction = null, string? headUrl = null);

        Task<bool> UpdateUserHeadUrl(string headUrl);

        Task<UserInfoDto> GetUserInfoAsync(string userId);

        Task<bool> UpdateUserPasswordAsync(string userId, string password);

        Task<UsersDto> GetUsersAsync(int pageIndex, int pageSize, string? userName = null);

        Task<UsersDto> GetActiveAdminUsersAsync(int pageIndex, int pageSize);

        Task<bool> AdminUpdateUserAsync(string userId, string? userName, string? email, string? password, long? roleId, string? headUrl = null);

        Task<bool> BanUserAsync(string UserId, int Status);
    }
}
