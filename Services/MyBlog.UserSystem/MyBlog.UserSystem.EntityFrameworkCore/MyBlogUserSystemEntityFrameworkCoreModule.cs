using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.EntityFrameworkCore;
using MyBlog.UserSystem.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace MyBlog.UserSystem.EntityFrameworkCore
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule),
               typeof(MyBlogUserSystemDomainModule))]
    public class MyBlogUserSystemEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
           
            context.Services.AddAbpDbContext<MyBlogUserSystemDbContext>(opts =>
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
