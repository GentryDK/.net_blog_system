using Ganss.Xss;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace MyBlog.BlogSystem.Domain
{
    [DependsOn(typeof(AbpDddDomainModule))]
    public class MyBlogSystemDomainModule:AbpModule
    {
        /// <summary>
        /// 这里面是写服务的注册的
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<HtmlSanitizer>();
            base.ConfigureServices(context);
        }
    }
}
