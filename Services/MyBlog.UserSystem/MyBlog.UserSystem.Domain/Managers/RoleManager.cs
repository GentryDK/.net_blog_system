using MyBlog.UserSystem.Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace MyBlog.UserSystem.Domain.Managers
{
    public class RoleManager: DomainService
    {
        public IRepository<Role> RoleRepo { get; set; }
        public IRepository<UserRoleMapping> UserRoleMappingRepo { get; }
        public IRepository<Permission> PermissionRepo {  get; set; }

        public RoleManager(
            IRepository<Role> roleRepo,
            IRepository<UserRoleMapping> userRoleMappingRepo,
            IRepository<Permission> permissionRepo
            ) 
        {
            this.RoleRepo = roleRepo;
            this.PermissionRepo = permissionRepo;
            this.UserRoleMappingRepo = userRoleMappingRepo;
        }

        /// <summary>
        /// 通过Id名称得到一个权限角色
        /// </summary>
        /// <returns></returns>
        public async Task<Role> GetRoleAsync(long roleId)
        {
           var role = await RoleRepo.GetAsync(m => m.Id == roleId);
            return role;
        }

        /// <summary>
        /// 通过userId名称得到当前角色的映射的角色
        /// </summary>
        /// <returns></returns>
        public async Task<UserRoleMapping> GetRoleMappingByUserIdAsync(string userId)
        {
            return await UserRoleMappingRepo.FirstOrDefaultAsync(m=>m.UserId == userId);
        }

        /// <summary>
        /// 通过userId名称得到当前角色的映射的角色
        /// </summary>
        /// <returns></returns>
        public async Task<Role> GetRoleByUserIdAsync(string userId)
        {
            var roleMapping = await UserRoleMappingRepo.FirstOrDefaultAsync(m => m.UserId == userId);
            if (roleMapping!=null)
            {
                var role = await RoleRepo.GetAsync(r => r.Id == roleMapping.RoleId);
                return role;
            }
            return null;
        }

        public async Task<List<string>> GetAdminUserIdAsync()
        {
            var roleMapping = await UserRoleMappingRepo.GetQueryableAsync();
            return await roleMapping.Where(r=>r.RoleId==2).Select(r=>r.UserId).ToListAsync();
        }

        /// <summary>
        /// 得到当前所有的权限角色
        /// </summary>
        /// <returns></returns>
        public async Task<List<Role>> GetAllRoleAsync()
        {
            return await RoleRepo.GetListAsync();
        }

        /// <summary>
        /// 得到当前userId是否可以访问对应的接口权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="constroller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<bool> GetPermissionAsync(string userId, string constroller,string action)
        {
            var res = await UserRoleMappingRepo.GetListAsync(m => m.UserId == userId);
            var roles = res.Select(ur => ur.RoleId).ToList();
           
            var hasPermission = await PermissionRepo.AnyAsync(p=> roles.Contains(p.RoleId)
            &&
           ( p.Controller ==constroller|| p.Controller == "ALL") 
            && 
            (p.Action == action || p.Action == "ALL")
            );
            return hasPermission;
        }

        /// <summary>
        /// 设置一个新的角色权限
        /// </summary>
        /// <returns></returns>
        public async Task<Role> InsertRoleAsync(Role role)
        {
           return await RoleRepo.InsertAsync(role);
        }

        /// <summary>
        /// 设置多个角色权限
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InsertRolesAsync(params Role[] roles)
        {
            await RoleRepo.InsertManyAsync(roles);
            return true;
        }

        /// <summary>
        /// 更新userRoleMapping
        /// </summary>
        /// <param name="roleMapping"></param>
        /// <returns></returns>
        public async Task<UserRoleMapping> UpdateRoleMappingAsync(UserRoleMapping roleMapping)
        {
           return await UserRoleMappingRepo.UpdateAsync(roleMapping);
        }

        /// <summary>
        /// 删除角色权限
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteRolesAsync(long roleId)
        {
            var roleMap = await UserRoleMappingRepo.GetAsync(m => m.RoleId == roleId);
            var rolePermission = await PermissionRepo.GetAsync(m => m.RoleId == roleId);
            if (roleMap == null || rolePermission == null)
            {
                return false;
            }
            await UserRoleMappingRepo.DeleteAsync(m => m.RoleId == roleId || m.Id == roleId); 
            await PermissionRepo.DeleteAsync(m => m.RoleId == roleId);
            return true;
        }
    }
}
