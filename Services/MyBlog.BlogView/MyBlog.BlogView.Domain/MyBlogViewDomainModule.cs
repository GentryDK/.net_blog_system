using Ganss.Xss;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace MyBlog.BlogView.Domain
{
    [DependsOn(typeof(AbpDddDomainModule))]
    public class MyBlogViewDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<HtmlSanitizer>();
            base.ConfigureServices(context);
        }
    }
}
