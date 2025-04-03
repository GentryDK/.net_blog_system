using MyBlog.UserSystem.Domain.Account;
using MyBlog.UserSystem.Domain.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.UserSystem.Application.Extensions
{
    public static class UserHelper
    {
        /// <summary>
        /// 更新用户的基本信息
        /// </summary>
        public static void UpdateUserBasicInfo(User user, string? userName, string? email, string? headUrl, string introduction=null)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                user.UserName = userName;
            }
            if (!string.IsNullOrEmpty(email))
            {
                user.Email = email;
            }
            if (!string.IsNullOrEmpty(headUrl))
            {
                user.HeadUrl = headUrl;
            }
            if (!string.IsNullOrEmpty(introduction))
            {
                user.Introduction = introduction;
            }
        }

        /// <summary>
        /// 更新用户的角色信息
        /// </summary>
        public static async Task UpdateUserRoleAsync(string userId, long? roleId, RoleManager roleManager)
        {
            if (roleId.HasValue)
            {
                var roleMapping = await roleManager.GetRoleMappingByUserIdAsync(userId);
                if (roleMapping != null)
                {
                    roleMapping.RoleId = roleId.Value;
                    await roleManager.UpdateRoleMappingAsync(roleMapping);
                }
            }
        }

        /// <summary>
        /// 更新用户的密码
        /// </summary>
        public static async Task UpdateUserPasswordAsync(string userId, string? password, UserManager userManager)
        {
            if (!string.IsNullOrEmpty(password))
            {
                await userManager.UpdateUserPasswordAsync(userId, password);
            }
        }
    }
}
