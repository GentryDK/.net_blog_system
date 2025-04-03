using MyBlog.UserSystem.Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.UserSystem.Domain.Share
{
    /// <summary>
    /// 需要存到缓存中的实体
    /// </summary>
    public class UserRefreshToken
    {
        public User User { get; set; }

        public string Token {  get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }
    }
}
