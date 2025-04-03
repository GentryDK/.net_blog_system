using MyBlog.UserSystem.Application.Contract.RoleApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application.Contract.UserApp.Dto
{
    public class AdminUpdateUserDto
    {
        public string UserId { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public long? RoleId { get; set; }
    }
}
