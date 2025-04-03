using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.UserSystem.Domain.Share
{
    /// <summary>
    /// RefreshToken用一段加密字符串表示用户
    /// </summary>
    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }
    }
}
