using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace MyBlog.BlogSystem.Application.Contract
{
    [DependsOn(typeof(AbpDddApplicationContractsModule))]
    public class MyBlogSystemApplicationContractModule:AbpModule
    {

    }
}
