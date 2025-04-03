using Microsoft.EntityFrameworkCore;
using MyBlog.UserSystem.Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MyBlog.UserSystem.Domain.Managers
{
    public class UserManager : DomainService
    {
        public IRepository<User> UserRepo { get; }
        public IRepository<Role> RoleRepo { get; }
        public IRepository<UserPassword> UserPasswordRepo { get;}
        public IRepository<UserRoleMapping> RoleMappingRepo { get;}

        public UserManager(
            IRepository<User> userRepo,
            IRepository<Role> roleRepo,
            IRepository<UserPassword> userPasswordRepo,
            IRepository<UserRoleMapping> roleMappingRepo
            ) 
        {
            this.UserRepo = userRepo;
            this.RoleRepo = roleRepo;
            this.UserPasswordRepo = userPasswordRepo;
            this.RoleMappingRepo = roleMappingRepo;
        }

        /// <summary>
        /// 判断用户的邮箱是否存在
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public async Task<bool> HasEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }
            if (UserRepo == null)
            {
                throw new InvalidOperationException("UserRepo is not initialized.");
            }
            return await UserRepo.AnyAsync(m => m.Email == email);
        }

        /// <summary>
        /// 获取到User
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetUserAsync(params string[] email)
        {
            //将传递过来的email的用户都从数据库中取出来
            var users = await UserRepo.GetListAsync(m=>email.Contains(m.Email));
            return await GetUserInfo(users);
        }

        /// <summary>
        /// 获取完整的用户信息
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        private async Task<List<User>> GetUserInfo(List<User> users)
        {
            foreach (var user in users)
            {
                var userId = user.Id;
                var userRoles = await RoleMappingRepo.GetListAsync(m => userId == m.UserId);
                var rolesId = userRoles.Select(m => m.RoleId);
                var role = await RoleRepo.GetAsync(m => rolesId.Contains(m.Id));
                user.Role = role;
            }
            return users;
        }

        /// <summary>
        /// 通过email去拿到密码，和传递来的password比较
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> GetLoginUser(string email, string password)
        {
            User? user = await UserRepo.GetAsync(m => m.Email == email);
            if (user != null)
            {
                var userPassword = await UserPasswordRepo.GetAsync(p => p.UserId == user.Id && p.IsDisuse == "F");
                var isPwdRight = await IsPasswordRight(password,userPassword.Password);
                if (isPwdRight)
                {
                    return GetUserInfo(new List<User> { user}).Result.FirstOrDefault();
                }
            }
            return null;
        }

        public async Task<bool> IsPasswordRight(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password,passwordHash);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> InsertUserAsync(User user)
        {
            try
            {
               user = await UserRepo.InsertAsync(user);
               user.UserPassword = await UserPasswordRepo.InsertAsync(user.UserPassword);
               await RoleMappingRepo.InsertAsync(new UserRoleMapping()
                {
                    UserId = user.Id,
                    RoleId = 1
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
               return await UserRepo.GetAsync(m => m.Id == userId);
            }
            return null;
        }

        public async Task<List<User>> GetActiveUsersByIdsAsync(List<string> userIds,int skip, int pageSize)
        {
            var query = await UserRepo.GetQueryableAsync();
            var resList = await query.Where(u=>userIds.Contains(u.Id) && u.Status==0).Skip(skip).Take(pageSize).ToListAsync();
            return resList;
        }

        public async Task<(List<User>,int)> GetUsersAsync(int skip, int pageSize, string? userName = null)
        {
            var query = await UserRepo.GetQueryableAsync();

            var totalCount = await query.CountAsync();

            // 如果 userName 不为空，则添加筛选条件
            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(m => m.UserName.Contains(userName));
            }
            var resList = await query.Skip(skip).Take(pageSize).ToListAsync();

            return (resList, totalCount);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var result = await UserRepo.UpdateAsync(user);
            return result != null;
        }

        /// <summary>
        /// 更改用户头像的URL
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateUserHeadUrl(string headUrl)
        {
            var headName = Path.GetFileNameWithoutExtension(headUrl);
            var user = await UserRepo.GetAsync(m => m.Id == headName);
            if (user != null)
            {
                user.HeadUrl = headUrl;
                var result = await UserRepo.UpdateAsync(user);
                return result != null;
            }
            return false;
        }

        /// <summary>
        /// 修改用户的密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserPasswordAsync(string userId, string password)
        {
            var userRes = await UserPasswordRepo.GetAsync(u => u.UserId == userId);
            if (userRes == null) return false;

            var passRes = await IsPasswordRight(password, userRes.Password);
            if (passRes) return false;

            userRes.Password = BCrypt.Net.BCrypt.HashPassword(password);
            await UserPasswordRepo.UpdateAsync(userRes);
            return true;
        }
    }
}
