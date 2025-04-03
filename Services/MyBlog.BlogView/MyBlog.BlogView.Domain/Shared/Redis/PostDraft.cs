using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Domain.Shared.Redis
{
    /// <summary>
    /// 需要存到缓存中的实体
    /// </summary>
    public class PostDraft
    {
        public string? Id { get; set; }

        /// <summary>
        /// 帖子标题
        /// </summary>
        public string PostTitle { get; set; }

        /// <summary>
        /// 帖子的概要
        /// </summary>
        public string? summary { get; set; }

        /// <summary>
        /// 帖子内容
        /// </summary>
        public string PostContent { get; set; }

        /// <summary>
        /// 该帖子属于的板块
        /// </summary>
        public string? PostTypeId { get; set; }

        /// <summary>
        /// 该帖子所属于的板块名称
        /// </summary>
        public string? PostTypeName { get; set; }

        /// <summary>
        /// 帖子是否容许讨论
        /// </summary>
        public string? Discuss { get; set; }

        /// <summary>
        /// 帖子的状态 0:发布 1:草稿
        /// </summary>
        public int? State { get; set; }
    }
}
