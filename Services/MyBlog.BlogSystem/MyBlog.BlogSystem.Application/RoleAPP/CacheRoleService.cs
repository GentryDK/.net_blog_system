using MyBlog.BlogSystem.Domain.Shared.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using MyBlog.UserSystem.Application.Contract.RoleApp;
using Microsoft.Extensions.Caching.Distributed;
using AngleSharp.Text;
using MyBlog.BlogSystem.Application.Contract.RoleApp;

namespace MyBlog.BlogSystem.Application.RoleAPP
{
    public class CacheRoleService: ApplicationService, ICacheRoleService
    {
        private  IDistributedCache<string> _roleDistributedCache { get; set; }
        private IRoleService _roleService { get; set; }

        public CacheRoleService(
             IDistributedCache<string> roleDistributedCache,
            IRoleService roleService
            )
        {
            this._roleDistributedCache = roleDistributedCache;
            this._roleService = roleService;
        }

        /// <summary>
        /// 从缓存中查看是否是超级管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> IsAdminCacheAsync(string userId)
        {
            var isRoleAdmin = await _roleDistributedCache.GetAsync("isSuperAdmin:"+userId);
            if (isRoleAdmin == null)
            {
               var isSuperAdmin = await _roleService.IsAdminAsync(userId);
                //配置过期时间
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                await _roleDistributedCache.SetAsync("isSuperAdmin:" + userId, isSuperAdmin.ToString(), cacheOptions);
                return isSuperAdmin;
            }
            return bool.Parse(isRoleAdmin);
        }

        public async Task<bool> HasPermissionCacheAsync(string userId, string controller, string action)
        {
            var isRoleAdmin = await _roleDistributedCache.GetAsync("isAdmin:" + userId + ":" + controller);
            if (isRoleAdmin == null)
            {
                var isAdmin = await _roleService.HasPermissionAsync(userId,controller,action);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                await _roleDistributedCache.SetAsync("isAdmin:" + userId+":"+controller, isAdmin.ToString(), cacheOptions);
                return isAdmin;
            }
            return bool.Parse(isRoleAdmin);
        }
    }
}
