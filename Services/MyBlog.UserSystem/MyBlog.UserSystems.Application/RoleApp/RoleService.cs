using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyBlog.UserSystem.Application.Contract.RoleApp;
using MyBlog.UserSystem.Application.Contract.RoleApp.Dtos;
using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;
using MyBlog.UserSystem.Domain.Account;
using MyBlog.UserSystem.Domain.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.UserSystem.Application.RoleApp
{
    public class RoleService : ApplicationService, IRoleService
    {
        public RoleManager RoleMgr { get; }
        public IConfiguration _configuration { get; }

        public RoleService(RoleManager roleMgr,IConfiguration configuration)
        {
            this.RoleMgr = roleMgr;
            this._configuration = configuration;
        }


        public async Task<List<Role>> GetAllRoleAsync()
        {
            return await RoleMgr.GetAllRoleAsync();
        }

        public async Task<bool> HasPermissionAsync(string userId,string controllerName,string actionName)
        {
           return await RoleMgr.GetPermissionAsync(userId, controllerName, actionName);
        }

        public async Task<bool> IsAdminAsync(string userId)
        {
            string superAdministratorName = _configuration["SuperAdministratorName"];
            var userRole = await RoleMgr.GetRoleMappingByUserIdAsync(userId);
            if (userRole != null) {
                var role = await RoleMgr.GetRoleAsync(userRole.RoleId);
                return role.RoleName == superAdministratorName;
            }
            return false;
        }

        public async Task<bool> RemoveRoleAsync(long roleId)
        {
            return await RoleMgr.DeleteRolesAsync(roleId);
        }


        public async Task<Role> InsertRoleAsync(RoleDto roleInput)
        {
            var role = ObjectMapper.Map<RoleDto, Role>(roleInput);
            return await RoleMgr.InsertRoleAsync(role);
        }
    }
}
