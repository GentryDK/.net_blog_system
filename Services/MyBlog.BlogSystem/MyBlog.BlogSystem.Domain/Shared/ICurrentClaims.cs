using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MyBlog.BlogSystem.Domain.Shared
{
    public interface ICurrentClaims : IScopedDependency
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string HeadUrl { get; set; }

        public bool IsSuperAdmin { get; set; }

        void TransClaims(IEnumerable<Claim> claims);

        void SetSuperAdmin(bool superAdim);
    }
}
