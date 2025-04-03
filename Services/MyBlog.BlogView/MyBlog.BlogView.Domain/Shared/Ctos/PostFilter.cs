using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Domain.Shared.Ctos
{
    public class PostFilter
    {
        public int pageIndex { get; set; }

        public int pageSize { get; set; }

        /// <summary>
        /// 帖子标题
        /// </summary>
        public string? PostTitle { get; set; }

        /// <summary>
        /// 该帖子属于的板块
        /// </summary>
        public string? PostTypeId { get; set; }
    }
}
