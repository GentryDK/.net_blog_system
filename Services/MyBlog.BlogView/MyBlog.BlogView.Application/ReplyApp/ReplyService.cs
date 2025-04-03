using MyBlog.BlogSystem.Domain.Managers;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogView.Application.Contract.ReplyApp;
using MyBlog.BlogView.Application.Contract.ReplyApp.Dto;
using MyBlog.BlogView.Application.Contract.SensitiveApp;
using MyBlog.BlogView.Application.SensitiveApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;

namespace MyBlog.BlogView.Application.ReplyApp
{
    public class ReplyService : ApplicationService, IReplyService
    {
        private ReplyManager _replyManager {  get; set; }
        private SensitiveService _sensitiveService { get; set; }

        public ReplyService(ReplyManager replyManager, SensitiveService sensitiveService) 
        {
            this._replyManager = replyManager;
            this._sensitiveService = sensitiveService;
        }

        public async Task<ReplyInfoDto> GetPostReplyAsync(int postReplyIndex, int postReplySize, string postId, int commentReplyIndex = 1, int commentReplySize = 2)
        {
            var replyInfoDto = new ReplyInfoDto();
            var postReplySkip = (postReplyIndex - 1) * postReplySize;
            var commentReplySkip = (commentReplyIndex - 1) * commentReplySize;

            // 1. 获取帖子下的评论（不包含回复）
            var (postReplies, postReplyCount) = await _replyManager.GetPostRepliesAsync(postId, postReplySkip, postReplySize);
            var postReplyDtos = ObjectMapper.Map<List<Reply>, List<ReplyDto>>(postReplies);

            postReplyDtos = postReplyDtos.OrderByDescending(p=>p.CreationTime).ToList();

            // 2. 获取每个评论的回复，并递归填充
            foreach (var replyDto in postReplyDtos)
            {
                var (replies, count) = await _replyManager.GetRepliesByQuoteReplyIdAsync(replyDto.Id, commentReplySkip, commentReplySize);
                var replyDtos = ObjectMapper.Map<List<Reply>, List<ReplyDto>>(replies);
                replyDto.CommentReplyCount = count;
                replyDto.replyDtos = replyDtos;
            }

            // 3. 返回数据
            replyInfoDto.ReplyDtos = postReplyDtos;
            replyInfoDto.PostReplyCount = postReplyCount;
            return replyInfoDto;
        }


        public async Task<ReplyInfoDto> GetCommentReplyAsync(int commentReplyIndex, int commentReplySize, string replyId)
        {
            var commentReplySkip = (commentReplyIndex - 1) * commentReplySize;
            var (list, count) = await _replyManager.GetRepliesByQuoteReplyIdAsync(replyId, commentReplySkip, commentReplySize);

            return new ReplyInfoDto
            {
                ReplyDtos = ObjectMapper.Map<List<Reply>, List<ReplyDto>>(list),
                PostReplyCount = count
            };
        }

        public async Task AddPostReplyAsnyc(ReplyDto replyDto)
        {
            //截取前50个字符，并加上省略号
            if (!string.IsNullOrEmpty(replyDto.ReplyContent) && replyDto.ReplyContent.Length > 50)
            {
                replyDto.ReplyContent = replyDto.ReplyContent.Substring(0, 50) + "...";
            }

            //获取到敏感词
            var words = await _sensitiveService.GetWordsInCacheAsync();

            var reply = ObjectMapper.Map<ReplyDto,Reply>(replyDto);
            reply.FilterSensitiveWords(words);
           await _replyManager.AddReplyAsync(reply);
        }
    }
}
