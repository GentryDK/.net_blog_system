using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace MyBlog.UserSystem.Domain.Account
{
    public class Permission:Entity<string>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public long RoleId {  get; set; }

        /// <summary>
        /// 控制器名称
        /// </summary>
        public string Controller {  get; set; }

        /// <summary>
        /// 操作名称，类似Get Set
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 具体的 URL 路径，指定权限适用的 API 端点或资源路径
        /// </summary>
        public string Url { get; set; }
    }
}
