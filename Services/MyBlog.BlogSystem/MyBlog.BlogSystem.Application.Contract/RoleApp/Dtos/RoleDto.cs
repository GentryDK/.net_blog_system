﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.UserSystem.Application.Contract.RoleApp.Dtos
{
    public class RoleDto
    {
        public int Id { get; set; }

        public string RoleName { get; set; }

        public int RoleLevel { get; set; }
    }
}
