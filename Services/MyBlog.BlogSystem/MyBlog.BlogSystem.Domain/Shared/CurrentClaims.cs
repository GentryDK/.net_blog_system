using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Domain.Shared
{
    public class CurrentClaims: ICurrentClaims
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string HeadUrl { get; set; }

        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// 初始化CurrentClaims信息的
        /// </summary>
        /// <param name="claims"></param>
        public void TransClaims(IEnumerable<Claim> claims)
        {
            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case "userId":
                        {
                            UserId = claim.Value;
                            break;
                        }
                    case "userName":
                        {
                            UserName = claim.Value;
                            break;
                        }
                    case "userUrl":
                        {
                            HeadUrl = claim.Value;
                            break;
                        }
                }
            }

            IsSuperAdmin = false;
        }

        public void SetSuperAdmin(bool superAdim)
        {
            this.IsSuperAdmin = superAdim;
        }
    }
}
