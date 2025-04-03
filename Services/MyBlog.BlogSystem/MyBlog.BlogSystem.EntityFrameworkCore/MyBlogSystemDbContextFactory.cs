using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.EntityFrameworkCore
{
    public class MyBlogSystemDbContextFactory : IDesignTimeDbContextFactory<MyBlogSystemDbContext>
    {
        public MyBlogSystemDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();
            var builder = new DbContextOptionsBuilder<MyBlogSystemDbContext>()
                .UseMySql(configuration.GetConnectionString("Default"), new MySqlServerVersion(new Version(8, 0, 36)));

            return new MyBlogSystemDbContext(builder.Options);
        }

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MyBlog.BlogSystem/"))
                .AddJsonFile("appsettings.json", false);
            return builder.Build();
        }
    }
}
