using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MyBlog.UserSystem.Application.Contract.UserApp.Dtos
{
    public class UserDto : EntityDto<string>
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 用户是否被封禁
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户角色表(这里是list表示一个用户有多个角色)
        /// </summary>
        [NotMapped]
        public List<string> Roles { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
