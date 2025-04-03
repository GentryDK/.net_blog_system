using Abp.AspNet.JwpBearer;
using MyBlog.UserSystem.Application.Contract.UserApp;
using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;
using MyBlog.UserSystem.Domain.Account;
using MyBlog.UserSystem.Domain.Managers;
using System.Security.Claims;
using Volo.Abp.Caching;
using Volo.Abp.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using MyBlog.UserSystem.Domain.Share;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using MyBlog.UserSystem.Application.Contract.RoleApp.Dtos;
using MyBlog.UserSystem.Application.Extensions;
using Abp.EMail;
using System.Text;
using System.Security.Cryptography;
using UserSystem.Application.Contract.UserApp.Dtos;

namespace MyBlog.UserSystem.Application.UserApp
{
    public class UserService:ApplicationService,IUserService
    {
        public HttpContext HttpContext { get; set; }

        public UserManager UserMgr { get; set; }
        public RoleManager RoleMgr { get; set; }
        public IEMailSender EMailSendder { get; }
        public TokenCreateModel TokenCreateModel { get; set; }

        public IDistributedCache<UserRefreshToken> UserRefreshTokenDistributedCache { get; set; }

        //用于邮箱验证的缓存
        public IDistributedCache CodeCache { get; }

        public UserService(
            UserManager userMgr,
            RoleManager roleMgr,
            TokenCreateModel tokenCreateModel,
             IDistributedCache codeCache,
             IEMailSender eMailSendder,
             IDistributedCache<UserRefreshToken> userRefreshTokenDistributedCachedistributedCache,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this.UserMgr = userMgr;
            this.RoleMgr = roleMgr;
            this.TokenCreateModel = tokenCreateModel;
            this.CodeCache = codeCache;
            this.EMailSendder= eMailSendder;
            this.UserRefreshTokenDistributedCache = userRefreshTokenDistributedCachedistributedCache;
            this.HttpContext = httpContextAccessor.HttpContext;
        }

        public async Task<string> CheckLoginAsync(string email,string passward)
        {
            string token = string.Empty;
            var user = await UserMgr.GetLoginUser(email,passward);
            if (user != null) 
            {
                token = CreateToken(user);

               var refreshToken = GenerateRefreshToken();
               SetRefreshToken(refreshToken, user);
            }
            return token;
        }

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="createInput"></param>
        /// <returns></returns>
        public async Task<bool> SendEmailCode(EMailDto emailDto)
        {
            Random rad = new Random();
            int value = rad.Next(1000, 10000);
            byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
            //将邮箱验证码存入redis中
            await CodeCache.SetAsync($"EMailCode:{emailDto.UserName}", bytes, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(3)
            });

            await EMailSendder.SendMail(new SendInfo
            {
                receiveUser = emailDto.Email,
                sendMessage = $"登录码:{value}",
                Subject = "验证登录码",
                isBodyHtml = false
            });
            return true;
        }

        public async Task<bool> RegistUserAsync(UserCreateDto createInput)
        {
            if (await UserMgr.HasEmailAsync(createInput.Email))
            {
                Console.WriteLine($"邮箱已注册: {createInput.Email}");
                return false;
            }

            // 从 Redis 获取验证码
            string emailCodeString = await CodeCache.GetStringAsync($"EMailCode:{createInput.UserName}");
            if (string.IsNullOrEmpty(emailCodeString))
            {
                Console.WriteLine($"验证码不存在或已过期: {createInput.UserName}");
                return false;
            }

            if (!string.Equals(emailCodeString, createInput.EmailCode, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"验证码错误: {createInput.UserName}");
                return false;
            }

            // 验证通过后，立即删除验证码（防止重复使用）
            await CodeCache.RemoveAsync($"EMailCode:{createInput.UserName}");

            // 映射用户对象
            var user = ObjectMapper.Map<UserCreateDto, User>(createInput);
            user.Init(new Role()
            {
                RoleLevel = 1,
                RoleName = "普通用户"
            }, createInput.Password);

            // 插入用户到数据库
            await UserMgr.InsertUserAsync(user);
            Console.WriteLine($"用户注册成功: {createInput.UserName}");
            return true;
        }

        /// <summary>
        /// 生成RefreshToken
        /// </summary>
        /// <returns></returns>
        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                //过期时间
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };
            return refreshToken;
        }

        /// <summary>
        /// 将RefreshToken生成到cookies中,同时把UserRefreshToken存入缓存中
        /// </summary>
        private async void SetRefreshToken(RefreshToken refrshToken,User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refrshToken.Expires,
            };
            HttpContext.Response.Cookies.Append("refreshToken",refrshToken.Token, cookieOptions);

            UserRefreshToken userRefreshToken = new UserRefreshToken()
            {
                User = user,
                Token = refrshToken.Token,
                Expires = refrshToken.Expires,
                Created = refrshToken.Created
            };

            await UserRefreshTokenDistributedCache.SetAsync(refrshToken.Token, userRefreshToken);
        }

        /// <summary>
        /// 滑动过期的方法，重新生成token给前端(如果token已经过期则不能用,如果token没过期才能延长时间)
        /// </summary>
        /// <returns></returns>
        public async Task<(string token,bool isSuccess)> RefreshToken()
        {
            var claims = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = claims.Value;

            var cookieRefreshToken = HttpContext.Request.Cookies["refreshToken"];
            var value = await UserRefreshTokenDistributedCache.GetAsync(cookieRefreshToken);
            if (value!=null)
            {
                var userRefreshToken = value;
                if (userRefreshToken.Token!=cookieRefreshToken)
                {
                    return ("refreshToken 验证失败",false);
                }
                else if (userRefreshToken.Expires<DateTime.Now)
                {
                    return ("refreshToken 已过期",false);
                }
                var token = TokenCreateModel.GetToken(userId,
                   new Claim("userName", userRefreshToken.User.UserName),
                   new Claim("userUrl", userRefreshToken.User.HeadUrl),
                   new Claim("userRoleLevel", userRefreshToken.User.Role.RoleLevel.ToString())
                   );
                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken,userRefreshToken.User);
                return (token,true);
            }
            return ("当前用户不存在",false);
        }

        private string CreateToken(User user)
        {
            var token = TokenCreateModel.GetToken(user.Id,
                   new Claim("userName", user.UserName),
                   new Claim("userUrl", user.HeadUrl),
                   new Claim("userRoleLevel", user.Role.RoleLevel.ToString())
                   );
            return token;
        }

        public async Task<bool> UpdateUserAsync(string userId, string? userName = null, string? email = null,string introduction = null, string? headUrl = null)
        {
            var user = await UserMgr.GetUserByIdAsync(userId);
            if (user != null)
            {
                UserHelper.UpdateUserBasicInfo(user, userName, email, headUrl,introduction);
                return await UserMgr.UpdateUserAsync(user);
            }
            return false;
        }

        public async Task<bool> AdminUpdateUserAsync(string userId, string? userName, string? email, string? password, long? roleId, string? headUrl = null)
        {
            var user = await UserMgr.GetUserByIdAsync(userId);
            if (user != null)
            {
                UserHelper.UpdateUserBasicInfo(user, userName, email, headUrl);
                await UserHelper.UpdateUserPasswordAsync(userId, password, UserMgr);
                await UserHelper.UpdateUserRoleAsync(userId, roleId, RoleMgr);

                return await UserMgr.UpdateUserAsync(user);
            }
            return false;
        }

        public async Task<UserInfoDto> GetUserInfoAsync(string userId)
        {
            var user = await UserMgr.GetUserByIdAsync(userId);
            var role = await RoleMgr.GetRoleByUserIdAsync(userId);
            if (user == null || role == null)
            {
                return null;
            }
            var roleDto = ObjectMapper.Map<Role,RoleDto>(role);
            var userInfo = ObjectMapper.Map<User, UserInfoDto>(user);
            userInfo.role= roleDto;
            return userInfo;
        }

        private async Task<List<UserInfoDto>> GetUserInfoDtosWithRolesAsync(List<User> users)
        {
            var userInfoDtos = ObjectMapper.Map<List<User>, List<UserInfoDto>>(users);
            foreach (var userInfoDto in userInfoDtos)
            {
                var role = await RoleMgr.GetRoleByUserIdAsync(userInfoDto.UserId);
                var roleDto = ObjectMapper.Map<Role, RoleDto>(role);
                userInfoDto.role = roleDto;
            }
            return userInfoDtos;
        }

        public async Task<UsersDto> GetUsersAsync(int pageIndex, int pageSize, string? userName = null)
        {
            UsersDto usersDto = new UsersDto();
            var skip = (pageIndex - 1) * pageSize;
            var (users, count) = await UserMgr.GetUsersAsync(skip, pageSize, userName);
            usersDto.Count = count;

            var userInfoDtos = await GetUserInfoDtosWithRolesAsync(users);
            usersDto.userInfoDtos = userInfoDtos;

            return usersDto;
        }

        public async Task<UsersDto> GetActiveAdminUsersAsync(int pageIndex, int pageSize)
        {
            UsersDto usersDto = new UsersDto();
            var skip = (pageIndex - 1) * pageSize;
            var adminUserList = await RoleMgr.GetAdminUserIdAsync();
            var users = await UserMgr.GetActiveUsersByIdsAsync(adminUserList, skip, pageSize);
            usersDto.Count = adminUserList.Count;

            var userInfoDtos = await GetUserInfoDtosWithRolesAsync(users);
            usersDto.userInfoDtos = userInfoDtos;

            return usersDto;
        }

        public async Task<bool> BanUserAsync(string UserId,int Status)
        {
            if (UserId!=null)
            {
                var user = await UserMgr.GetUserByIdAsync(UserId);
                user.Status = Status;
                var result = await UserMgr.UpdateUserAsync(user);
                return result;
            }
            return false;
        }

        public async Task<bool> UpdateUserHeadUrl(string headUrl)
        {
           return await UserMgr.UpdateUserHeadUrl(headUrl);
        }

        public async Task<bool> UpdateUserPasswordAsync(string userId,string password)
        {
           return await UserMgr.UpdateUserPasswordAsync(userId,password);
        }
    }
}
