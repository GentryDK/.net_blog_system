using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MyBlog.BlogSystem.Domain.Managers
{
    public class ReplyManager : DomainService
    {
        private readonly IRepository<Reply> ReplyRepo;
        private readonly IRepository<Post> PostRepo;

        //public IQueryable<Reply> ReplyQueryable => ReplyRepo.GetQueryableAsync().Result;
        public ICurrentClaims CurrentClaims;

        //防止xss攻击的包，需要在nuget引用一下,然后添加进容器后直接使用即可
        public readonly HtmlSanitizer HtmlSanitizer;

        public ReplyManager(
            IRepository<Reply> replyRepo,
            IRepository<Post> postRepo,
            ICurrentClaims currentClaims,
            HtmlSanitizer htmlSanitizer
            )
        {
            this.HtmlSanitizer = htmlSanitizer;
            this.CurrentClaims = currentClaims;
            this.ReplyRepo = replyRepo;
            this.PostRepo = postRepo;
        }

        /// <summary>
        /// 获取帖子评论列表和总数
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<(List<Reply>, int)> GetPostRepliesAsync(string postId, int skip, int take)
        {
            if (postId == null)
            {
                throw new ArgumentNullException(nameof(postId));
            }
            return await GetRepliesAsync(r => r.PostId == postId && (r.QuoteReplyId == null || r.QuoteReplyId == ""), skip, take);
        }


        /// <summary>
        /// 获取某个评论的回复列表
        /// </summary>
        /// <param name="QuoteReplyId"></param>
        /// <returns></returns>
        public async Task<(List<Reply>, int)> GetRepliesByQuoteReplyIdAsync(string quoteReplyId, int skip, int take)
        {
            if (quoteReplyId == null)
            {
                throw new ArgumentNullException(nameof(quoteReplyId));
            }
            return await GetRepliesAsync(r => r.QuoteReplyId == quoteReplyId, skip, take);
        }

        public async Task<(List<Reply>, int)> GetAllRepliesByPostIdAsync(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentNullException(nameof(postId));
            }

            var query = await ReplyRepo.GetQueryableAsync();
            var replies = await query.Where(r => r.PostId == postId)
                                     .OrderBy(m => m.CreationTime)
                                     .ToListAsync();

            var count = replies.Count(r => r.QuoteReplyId == null || r.QuoteReplyId == "");
            return (replies, count);
        }

        public async Task<(List<Reply>, int)> GetRepliesAsync(Expression<Func<Reply, bool>> predicate, int skip, int take)
        {
            var query = await ReplyRepo.GetQueryableAsync();
            var replies = await query.Where(predicate)
                                      .Skip(skip)
                                      .Take(take)
                                      .OrderByDescending(m => m.CreationTime)
                                      .ToListAsync();

            var count = await query.Where(predicate).CountAsync();
            return (replies, count);
        }

        /// <summary>
        /// 添加回复信息
        /// </summary>
        /// <param name="reply"></param>
        /// <returns></returns>
        public async Task<Reply> AddReplyAsync(Reply reply)
        {
            reply.InitReply(CurrentClaims,reply.UserName,reply.HeadUrl);
            //过滤回帖的内容（就是这里如果回复内容涵盖dom元素标签，会自动去掉）
            reply.ReplyContent = HtmlSanitizer.Sanitize(reply.ReplyContent);
            var isSuccess = await ReplyRepo.InsertAsync(reply);
            return isSuccess;
        }
    }
}
