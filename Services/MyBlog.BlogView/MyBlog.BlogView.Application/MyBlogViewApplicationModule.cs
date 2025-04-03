using DistributedLock;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.BlogSystem.EntityFrameworkCore;
using MyBlog.BlogView.Application.Contract;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace MyBlog.BlogView.Application
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(MyBlogViewApplicationContractModule),
        typeof(MyBlogViewEntityFrameworkCoreModule)
    )]
    public class MyBlogViewApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(opt =>
            {
                opt.AddProfile<MyBlogViewProfile>();
            });

            var configuration = context.Services.GetConfiguration();
            context.Services.AddSingleton<DistributedLockService>();
            base.ConfigureServices(context);
        }
    }
}
