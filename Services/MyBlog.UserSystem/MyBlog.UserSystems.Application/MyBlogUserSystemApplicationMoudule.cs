using Abp.EMail;
using MyBlog.UserSystem.Application.Contract;
using MyBlog.UserSystem.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace MyBlog.UserSystem.Application
{
    [DependsOn(typeof(AbpDddApplicationModule),
               typeof(AbpAutoMapperModule),
                typeof(AbpEMailModule),
               typeof(MyBlogUserSystemEntityFrameworkCoreModule),
               typeof(MyBlogUserSystemApplicationContractModule)
        )]
    public class MyBlogUserSystemApplicationMoudule: AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(opt =>
            {
                opt.AddProfile<UserSystemProfile>();
            });
            base.ConfigureServices(context);
        }
    }
}
