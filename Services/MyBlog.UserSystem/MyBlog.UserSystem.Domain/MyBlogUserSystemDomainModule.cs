using Abp.EMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace MyBlog.UserSystem.Domain
{
    [DependsOn(typeof(AbpDddDomainModule),
               typeof(AbpEMailModule))]
    public class MyBlogUserSystemDomainModule : AbpModule
    {
        /// <summary>
        /// 这里面是写服务的注册的
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }

        /// <summary>
        /// 写入需要使用的中间键的(写在这里会自动帮我们添加到Program中)
        /// </summary>
        /// <param name="context"></param>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {

        }
    }
}
