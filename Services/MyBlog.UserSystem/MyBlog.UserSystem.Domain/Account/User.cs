using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace MyBlog.UserSystem.Domain.Account
{
    /// <summary>
    /// 用户的账号实体
    /// </summary>
    public class User : AggregateRoot<string>, IHasCreationTime
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
        /// 用户是否被封禁 1是封禁 0是启用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户简介
        /// </summary>
        public string? Introduction { get; set; }

        [NotMapped]
        public Role Role { get; set; }

        [NotMapped]
        public UserPassword UserPassword { get; set; }

        public DateTime CreationTime { get; set; }

        public User()
        {

        }

        public User(string id) : base(id)
        {
           
        }

        public void Init(Role role, string password)
        {
            this.CreationTime = DateTime.Now;
            this.Status = 0;
            RandomHeadUrl();
            CreateUser(role,password);
        }

        public void RandomHeadUrl()
        {
            this.HeadUrl = $"/img/head/head({new Random().Next(1, 9)}).png";
        }

        public void CreateUser(Role role,string password)
        {
            SetPassword(password);
            SetRole(role);
        }

            public void SetPassword(string password)
        {
            this.UserPassword = new UserPassword(password);
            this.UserPassword.CreationTime = this.CreationTime;
            this.UserPassword.UserId = this.Id;
            this.UserPassword.IsDisuse = "F";
        }

        public void SetRole(Role roleNow)
        {
            this.Role = roleNow;
        }
    }
}
