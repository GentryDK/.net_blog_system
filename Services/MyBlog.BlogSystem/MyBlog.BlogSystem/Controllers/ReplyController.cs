using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Application.Contract.ReplyApp;
using MyBlog.BlogSystem.Application.Contract.ReplyApp.Dto;
using MyBlog.BlogSystem.Application.PostApp;
using MyBlog.BlogSystem.Application.ReplyApp;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using MyBlog.BlogSystem.web.filters;

namespace MyBlog.BlogSystem.web.Controllers
{
    [ApiController]
    [Route("MyBlog/[controller]")]
    public class ReplyController : ControllerBase
    {
        private IReplyService _replyService {  get; set; }

        public ReplyController(IReplyService replyService)
        {
            this._replyService = replyService;
        }

        [HttpGet("GetReplies")]
        [Authorize]
        [PermissionFilter]
        public async Task<ReplyInfoDto> GetReplyInfoAsync(int index,int size,string postId)
        {
            return await _replyService.GetRepliesDtoByPostIdAsync(index,size,postId);
        }

        [HttpGet("DeleteReply")]
        [Authorize]
        [PermissionFilter]
        public async Task<bool> DelReplAsync(string replyId)
        {
            return await _replyService.DeleteReplyAsync(replyId);
        }
    }
}
