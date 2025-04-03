using MyBlog.BlogSystem.Application.Contract.UserApp.Dto;
using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.UserSystem.Application.Contract.UserApp
{
    public interface IUserService : IApplicationService
    {
        Task<bool> UpdateUserAsync(string userId, string? userName = null, string? email = null, string introduction = null, string? headUrl = null);

        Task<UserInfoDto> GetUserInfoAsync(string userId);

        Task<bool> UpdateUserPasswordAsync(string userId, string password);

        Task<UsersDto> GetUsersAsync(int pageIndex, int pageSize, string? userName = null);

        Task<bool> AdminUpdateUserAsync(string userId, string? userName, string? email, string? password, long? roleId, string? headUrl = null);

        Task<bool> BanUserAsync(string UserId, int Status);
    }
}
