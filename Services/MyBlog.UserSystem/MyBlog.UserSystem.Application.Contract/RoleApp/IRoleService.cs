using MyBlog.UserSystem.Application.Contract.RoleApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBlog.UserSystem.Domain.Account;
using Volo.Abp.Application.Services;


namespace MyBlog.UserSystem.Application.Contract.RoleApp
{
    public interface IRoleService : IApplicationService
    {
        Task<List<Role>> GetAllRoleAsync();

        Task<bool> RemoveRoleAsync(long roleId);

        Task<Role> InsertRoleAsync(RoleDto roleInput);

        Task<bool> HasPermissionAsync(string userId, string controllerName, string actionName);

        Task<bool> IsAdminAsync(string userId);
    }
}
