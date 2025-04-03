using MyBlog.BlogSystem.Application.Contract.ReplyApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyBlog.BlogSystem.Application.Contract.ReplyApp
{
    public interface IReplyService: IApplicationService
    {
        Task<ReplyInfoDto> GetRepliesDtoByPostIdAsync(int index, int size, string postId);

        Task<bool> DeleteReplyAsync(string id);
    }
}
