using Microsoft.AspNetCore.Mvc.Filters;
using MyBlog.BlogSystem.Domain.Shared;

namespace MyBlog.BlogSystem.web.filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CurrentUserAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly ICurrentClaims _currentClaims;

        public CurrentUserAuthorizationFilterAttribute(
            ICurrentClaims currentClaims
            )
        {
            _currentClaims = currentClaims;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var claims = context.HttpContext.User.Claims;
            _currentClaims.TransClaims(claims);
        }
    }
}
