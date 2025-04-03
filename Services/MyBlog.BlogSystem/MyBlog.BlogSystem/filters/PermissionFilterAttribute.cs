using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using MyBlog.UserSystem.Application.Contract.RoleApp;
using Volo.Abp.DependencyInjection;
using MyBlog.BlogSystem.Domain.Shared;
using MyBlog.BlogSystem.Application.Contract.RoleApp;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MyBlog.BlogSystem.web.filters
{
    //IAsyncActionFilter 是 ASP.NET Core 中用于定义异步操作过滤器（Action Filter）的接口
    //用于实现诸如日志记录、身份验证、权限检查、错误处理等跨领域的关注点
    public class PermissionFilterAttribute : Attribute, IAsyncActionFilter,ITransientDependency
    {
        //是否是超级管理员可以访问
        private readonly bool _superAdminOnly; 

        public PermissionFilterAttribute(
            bool superAdminOnly=false)
        {
            this._superAdminOnly = superAdminOnly;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            ////从容器中获取对应的IRoleService
            var roleCacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheRoleService>();
            var currentClaims = context.HttpContext.RequestServices.GetRequiredService<ICurrentClaims>();

            //查找条件，表示我们要找的是类型为 NameIdentifier 的声明
            //?.Value 是安全导航操作符，表示如果找到了声明，则获取其值（用户ID）；如果没有找到，则返回 null
            var userId = currentClaims.UserId;
            //var controllerName = context.ActionDescriptor.RouteValues["controller"];
            //var actionName = context.ActionDescriptor.RouteValues["action"];
            //var controllerName = context.ActionDescriptor.RouteValues["controller"];
            //currentClaims.UserId;

            // 获取 ControllerActionDescriptor
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            // 检查各种 HTTP 属性以获取模板
            var httpGetAttribute = actionDescriptor?.MethodInfo.GetCustomAttributes(typeof(HttpGetAttribute), false).FirstOrDefault() as HttpGetAttribute;
            var httpPostAttribute = actionDescriptor?.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).FirstOrDefault() as HttpPostAttribute;
            var httpPutAttribute = actionDescriptor?.MethodInfo.GetCustomAttributes(typeof(HttpPutAttribute), false).FirstOrDefault() as HttpPutAttribute;
            var httpDeleteAttribute = actionDescriptor?.MethodInfo.GetCustomAttributes(typeof(HttpDeleteAttribute), false).FirstOrDefault() as HttpDeleteAttribute;

            // 根据找到的属性获取 Template
            var controllerName = httpGetAttribute?.Template ??
                                 httpPostAttribute?.Template ??
                                 httpPutAttribute?.Template ??
                                 httpDeleteAttribute?.Template;

            var actionName = context.HttpContext.Request.Method;  // 获取 HTTP 请求方法 (GET, POST 等)

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
            {
                //终止当前请求并返回 HTTP 403 Forbidden 状态代码给客户端
                context.Result = new ForbidResult();
                return;
            }

            var isSuperAdmin = await roleCacheService.IsAdminCacheAsync(userId);

            if (_superAdminOnly && !isSuperAdmin) 
            {
                context.Result = new ForbidResult();
                return; 
            }
            if (!isSuperAdmin)
            {
                var hasPermission = await roleCacheService.HasPermissionCacheAsync(userId, controllerName, actionName);
                if (!hasPermission)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            currentClaims.SetSuperAdmin(isSuperAdmin);
            await next();
        }
    }
}
