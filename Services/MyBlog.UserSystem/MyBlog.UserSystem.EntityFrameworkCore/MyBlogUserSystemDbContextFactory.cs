using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.UserSystem.EntityFrameworkCore
{
    public class MyBlogUserSystemDbContextFactory : IDesignTimeDbContextFactory<MyBlogUserSystemDbContext>
    {
        public MyBlogUserSystemDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();
            var builder = new DbContextOptionsBuilder<MyBlogUserSystemDbContext>()
                .UseMySql(configuration.GetConnectionString("Default"), new MySqlServerVersion(new Version(8, 0, 36)));

            return new MyBlogUserSystemDbContext(builder.Options);
        }

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MyBlog.UserSystems/"))
                .AddJsonFile("appsettings.json", false);
            return builder.Build();
        }
    }
}
