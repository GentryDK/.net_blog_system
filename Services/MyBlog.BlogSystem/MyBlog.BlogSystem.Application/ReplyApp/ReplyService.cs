using MyBlog.BlogSystem.Application.Contract.ReplyApp;
using MyBlog.BlogSystem.Application.Contract.ReplyApp.Dto;
using MyBlog.BlogSystem.Domain.Managers;
using MyBlog.BlogSystem.Domain.PostInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.BlogSystem.Application.ReplyApp
{
    public class ReplyService: ApplicationService, IReplyService
    {
        private ReplyManager _replyManager {  get; set; }

        public ReplyService(ReplyManager replyManager)
        {
            this._replyManager = replyManager;
        }

        public async Task<ReplyInfoDto> GetRepliesDtoByPostIdAsync(int index,int size,string postId)
        {
            ReplyInfoDto replyInfo = new ReplyInfoDto();
            var skip = (index - 1) * size;
            var(replies, totalCount) = await _replyManager.GetReplysByPostIdAsync(skip,size, postId);
            var replyDtos = ObjectMapper.Map<List<Reply>, List<ReplyDto>>(replies);
            replyInfo.ReplyDtos = replyDtos;
            replyInfo.PostReplyCount = totalCount;
            return replyInfo;
        }

        public async Task<bool> DeleteReplyAsync(string id)
        {
          return await _replyManager.HardDeleteReplyAsync(id);
        }
    }
}
