using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace MyBlog.UserSystem.Domain.Account
{
    public class Role : Entity<long>
    {
        public string RoleName {  get; set; }

        public int RoleLevel {  get; set; }
    }
}
