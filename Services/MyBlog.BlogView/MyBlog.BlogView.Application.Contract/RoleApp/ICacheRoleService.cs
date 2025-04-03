using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.BlogSystem.Application.Contract.RoleApp
{
    public interface ICacheRoleService: IApplicationService
    {
        Task<bool> IsAdminCacheAsync(string userId);

        Task<bool> HasPermissionCacheAsync(string userId, string controller, string action);
    }
}
