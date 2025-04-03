using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.UserSystem.Application.Contract.UserApp.Dtos
{
    public class UserCreateDto
    {
        [Required]
        [MaxLength(10)]
        [MinLength(2)]
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        [Required(ErrorMessage = "email不能为空")]
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string EmailCode { get; set; }

    }
}
