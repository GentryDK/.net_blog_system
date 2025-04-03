using Microsoft.Extensions.DependencyInjection;
using MyBlog.BlogSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace MyBlog.BlogSystem.EntityFrameworkCore
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule),
               typeof(MyBlogSystemDomainModule))]
    public class MyBlogSystemEntityFrameworkCoreModule:AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MyBlogSystemDbContext>(opts =>
            {
                opts.AddDefaultRepositories(true);
            });
            Configure<AbpDbContextOptions>(opt =>
            {

                opt.UseMySQL();
            });

            base.ConfigureServices(context);
        }
    }
}
