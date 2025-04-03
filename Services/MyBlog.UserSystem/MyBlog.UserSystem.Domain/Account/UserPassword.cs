using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace MyBlog.UserSystem.Domain.Account
{
    public class UserPassword : Entity<string>, IHasCreationTime
    {
        public string Password { get; set; }

        /// <summary>
        /// 密码对应的用户Id
        /// </summary>
        public string UserId { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 该密码当前是否在使用
        /// </summary>
        public string IsDisuse { get; set; }

        public UserPassword() { }

        public UserPassword(string password)
        {
            this.Id = Guid.NewGuid().ToString("N");
            this.Password = BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
