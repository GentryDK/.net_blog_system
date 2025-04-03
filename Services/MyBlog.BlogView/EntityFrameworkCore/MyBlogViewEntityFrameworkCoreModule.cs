using MyBlog.BlogView.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace MyBlog.BlogView.EntityFrameworkCore
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule),
               typeof(MyBlogViewDomainModule))]
    public class MyBlogViewEntityFrameworkCoreModule:AbpModule
    {

    }
}
