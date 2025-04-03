using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application.Contract.UserApp.Dto
{
    /// <summary>
    /// 负责前端和BlogSystem后端更新user
    /// </summary>
    public class UpdateUserInputDto
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户简介
        /// </summary>
        public string? Introduction { get; set; }

        public IFormFile? HeadImgFile { get; set; }
    }
}
