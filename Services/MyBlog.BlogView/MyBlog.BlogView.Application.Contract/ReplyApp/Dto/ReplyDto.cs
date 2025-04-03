using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MyBlog.BlogView.Application.Contract.ReplyApp.Dto
{
    public class ReplyDto
    {
        public string? Id { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadUrl { get; set; }

        /// <summary>
        /// 回复的用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 当前回复属于哪个帖子下面的
        /// </summary>
        public string PostId { get; set; }

        /// <summary>
        /// 在当前帖子下面回复的另一个用户的回复
        /// </summary>
        public string? ReplyUserName { get; set; }

        /// <summary>
        /// 帖子的创建时间
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// 回复的修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 当前回复的帖子id
        /// </summary>
        public string? QuoteReplyId { get; set; }

        /// <summary>
        /// 当前评论下面的回复数量
        /// </summary>
        public int? CommentReplyCount { get; set; }

        /// <summary>
        /// //当前评论下面的回复
        /// </summary>
        public List<ReplyDto>? replyDtos { get; set; }
    }
}
