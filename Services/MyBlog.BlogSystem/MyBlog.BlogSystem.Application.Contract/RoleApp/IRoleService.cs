using MyBlog.UserSystem.Application.Contract.RoleApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.UserSystem.Application.Contract.RoleApp
{
    public interface IRoleService : IApplicationService
    {
        Task<bool> HasPermissionAsync(string userId, string controllerName, string actionName);

        Task<bool> IsAdminAsync(string userId);
    }
}
