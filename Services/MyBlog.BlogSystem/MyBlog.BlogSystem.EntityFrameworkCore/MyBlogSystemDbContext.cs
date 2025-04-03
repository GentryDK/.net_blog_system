using Microsoft.EntityFrameworkCore;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.SensitiveInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace MyBlog.BlogSystem.EntityFrameworkCore
{
    public class MyBlogSystemDbContext:AbpDbContext<MyBlogSystemDbContext>
    {
        public DbSet<PostType> PostType {  get; set; }
        public DbSet<Post> Post {  get; set; }
        public DbSet<Reply> Reply { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<SensitiveWordsLibrary> SensitiveWordsLibrary {  get; set; }

        public MyBlogSystemDbContext(DbContextOptions<MyBlogSystemDbContext> options):base(options)
        {

        }

        //如果vs的包管理控制台不能迁移，需要这里在再次配置一下
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=localhost;Database=MyBlogSystemDb;User=root;Password=z1230456;", new MySqlServerVersion(new Version(8, 0, 36)));
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
