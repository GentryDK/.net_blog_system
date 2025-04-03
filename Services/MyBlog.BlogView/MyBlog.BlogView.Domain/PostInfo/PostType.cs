using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace MyBlog.BlogSystem.Domain.PostInfo
{
    /// <summary>
    /// 帖子的类型
    /// </summary>
    public class PostType : Entity<string>
    {
        public PostType(string id) : base(id)
        {

        }

        public string? Cover { get; set; }

        /// <summary>
        /// 板块名称
        /// </summary>
        public string PostTypeName { get; set; }

        public string? TypeBrief { get; set; }

        /// <summary>
        /// 板块是否被删除
        /// </summary>
        public string IsDeleted { get; set; }

        public int Order { get; set; }

        public void InitPostType(int order)
        {
            if (TypeBrief == null)
            {
                TypeBrief = "这里什么都没有";
            }
            this.Id = Guid.NewGuid().ToString("N");
            IsDeleted = "F";
            Order = order;
        }
    }
}
