using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.UserSystem.Application.Contract.UserApp.Dtos
{
    public class UsersDto
    {
        public List<UserInfoDto> userInfoDtos { get; set; }

        public int Count { get; set; }
    }
}
