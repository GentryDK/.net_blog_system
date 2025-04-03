using DistributedLock;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.BlogSystem.Application.Contract;
using MyBlog.BlogSystem.Domain.AppSettings;
using MyBlog.BlogSystem.EntityFrameworkCore;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace MyBlog.BlogSystem.Application
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(MyBlogSystemEntityFrameworkCoreModule),
        typeof(MyBlogSystemApplicationContractModule)
    )]
    public class MyBlogSystemApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(opt =>
            {
                opt.AddProfile<MyBlogSystemProfile>();
            });

            var configuration = context.Services.GetConfiguration();

            context.Services.Configure<UploadSettings>(configuration.GetSection("UploadSettings"));
            context.Services.AddSingleton<DistributedLockService>();
            base.ConfigureServices(context);
        }
    }
}