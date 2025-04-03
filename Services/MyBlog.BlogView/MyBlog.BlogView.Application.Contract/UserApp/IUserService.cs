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
        Task<UsersDto> GetActiveAdminUsersAsync(int pageIndex, int pageSize);

        Task<UserInfoDto> GetUserInfoAsync(string userId);
    }
}
