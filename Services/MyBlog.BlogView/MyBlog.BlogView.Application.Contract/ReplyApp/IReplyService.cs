using MyBlog.BlogView.Application.Contract.ReplyApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.BlogView.Application.Contract.ReplyApp
{
    public interface IReplyService: IApplicationService
    {
        Task<ReplyInfoDto> GetPostReplyAsync(int postReplyIndex, int postReplySize, string postId, int commentReplyIndex = 1, int commentReplySize = 2);

        Task<ReplyInfoDto> GetCommentReplyAsync(int commentReplyIndex, int commentReplySize, string replyId);

        Task AddPostReplyAsnyc(ReplyDto replyDto);
    }
}
