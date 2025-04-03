using Microsoft.Extensions.DependencyInjection;
using MyBlog.UserSystem.Application.Contract.UserApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace MyBlog.BlogView.HttpApi.Client
{
    [DependsOn(typeof(AbpHttpClientModule))]
    public class BlogViewHttpClientModule:AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddHttpClientProxies(
            //    typeof(IRoleService).Assembly,
            //    remoteServiceConfigurationName: "UserSystem"
            //    );

            context.Services.AddHttpClientProxies(
                typeof(IUserService).Assembly,
                remoteServiceConfigurationName: "UserSystem"
                );

            base.ConfigureServices(context);
        }
    }
}
