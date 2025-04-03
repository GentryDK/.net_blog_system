using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using MyBlog.BlogSystem.Domain.Extensions;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MyBlog.BlogSystem.Domain.Managers
{
    public class ReplyManager : DomainService
    {
        private readonly IRepository<Reply> ReplyRepo;
        public ICurrentClaims CurrentClaims;
        public readonly HtmlSanitizer HtmlSanitizer;

        public ReplyManager(
            IRepository<Reply> replyRepo,
            ICurrentClaims currentClaims,
            HtmlSanitizer htmlSanitizer
            )
        {
            this.HtmlSanitizer = htmlSanitizer;
            this.CurrentClaims = currentClaims;
            this.ReplyRepo = replyRepo;
        }

        public async Task<(List<Reply>,int)> GetReplysByPostIdAsync(int skip,int size,string postId)
        {
            var query = await ReplyRepo.GetQueryableAsync();
            var totalCount = await query.CountAsync(reply => reply.PostId == postId);
            var replies = await query.Where(reply => reply.PostId == postId).OrderByDescending(reply => reply.CreationTime).Skip(skip).Take(size).ToListAsync();
            return (replies, totalCount);
        }

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> HardDeleteReplyAsync(string id)
        {
            await ReplyRepo.DeleteAsync(m => m.Id == id);
            return true;
        }
    }
}
