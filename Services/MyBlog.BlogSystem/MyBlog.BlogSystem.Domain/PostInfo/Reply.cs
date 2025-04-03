using MyBlog.BlogSystem.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace MyBlog.BlogSystem.Domain.PostInfo
{
    /// <summary>
    /// 回复
    /// </summary>
    public class Reply : Entity<string>
        , IHasCreationTime
        , IHasModificationTime
    {
        public Reply(string id) : base(id)
        {

        }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }

        /// <summary>
        /// 发回复的用户Id
        /// </summary>
        public string UserId { get; set; }

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
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 回复的修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 帖子删除时间
        /// </summary>
        public DateTime? DeletionTime { get; set; }

        /// <summary>
        /// 帖子是否被删除
        /// </summary>
        public string IsDeleted { get; set; }

        /// <summary>
        /// 当前回复的帖子id
        /// </summary>
        public string? QuoteReplyId { get; set; }

        /// <summary>
        /// 当前回复的人的id
        /// </summary>
        public string? QuoteReplyUserId { get; set; }

        /// <summary>
        /// 当前回复的内容是什么，存储需要回复的人的发帖内容
        /// </summary>
        public string? QuoteReplyContent { get; set; }

        public void InitReply(ICurrentClaims currentClaims)
        {
            this.Id = Guid.NewGuid().ToString("N");

            UserId = currentClaims.UserId;
            UserName = currentClaims.UserName;
            HeadUrl = currentClaims.HeadUrl;
            CreationTime = DateTime.Now;
            LastModificationTime = DateTime.Now;
            this.IsDeleted = "F";
        }
    }
}
