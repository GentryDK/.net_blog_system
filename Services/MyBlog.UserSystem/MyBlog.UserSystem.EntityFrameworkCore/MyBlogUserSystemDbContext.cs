using Microsoft.EntityFrameworkCore;
using MyBlog.UserSystem.Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace MyBlog.UserSystem.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class MyBlogUserSystemDbContext : AbpDbContext<MyBlogUserSystemDbContext>
    {
        public DbSet<User> User { get; set; }
        public DbSet<UserPassword> UserPassword { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRoleMapping> UserRoleMapping { get; set; }
        public DbSet<Permission> Permission { get; set; }

        public MyBlogUserSystemDbContext(DbContextOptions<MyBlogUserSystemDbContext> options) : base(options)
        {
        }

        //如果vs的包管理控制台不能迁移，需要这里在再次配置一下
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=localhost;Database=MyBlogUserSystemDb;User=root;Password=z1230456;", new MySqlServerVersion(new Version(8,0,36)));
            }
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 生成迁移日志用的
        /// </summary>
        public void Migrator()
        {
            this.Database.Migrate();
        }
    }
}
