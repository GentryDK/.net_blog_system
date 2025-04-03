using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBlog.UserSystem.Application.Contract.RoleApp.Dtos;

namespace MyBlog.UserSystem.Application.Contract.UserApp.Dtos
{
    /// <summary>
    /// userSystem和blogManager进行数据的交互
    /// </summary>
    public class UserInfoDto
    {
        public string UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        public int Status { get; set; }

        public RoleDto role { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 用户简介
        /// </summary>
        public string? Introduction { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? HeadUrl { get; set; }
    }
}
