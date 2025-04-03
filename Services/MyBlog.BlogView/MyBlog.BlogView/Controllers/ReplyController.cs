using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.BlogView.Application.Contract.ReplyApp.Dto;
using MyBlog.BlogView.Application.ReplyApp;

namespace MyBlog.BlogView.web.Controllers
{
    [Route("MyBlogView/[controller]")]
    [ApiController]
    public class ReplyController : ControllerBase
    {
        private ReplyService _replyService {  get; set; }

        public ReplyController(ReplyService replyService) 
        {
            this._replyService = replyService;
        }

        [HttpGet("GetPostReply")]
        public async Task<ReplyInfoDto> GetPostReplyAsync(int postReplyIndex, int postReplySize, string postId, int commentReplyIndex = 1, int commentReplySize = 5)
        {
          return await _replyService.GetPostReplyAsync(postReplyIndex,postReplySize,postId, commentReplyIndex, commentReplySize);
        }

        [HttpGet("GetCommentReply")]
        public async Task<ReplyInfoDto> GetCommentReplyAsync(int commentReplyIndex, int commentReplySize, string replyId)
        {
            return await _replyService.GetCommentReplyAsync(commentReplyIndex, commentReplySize, replyId);
        }

        [HttpPost("AddPostReply")]
        [Authorize]
        public async Task<IActionResult> AddPostReplyAsnyc([FromForm]ReplyDto replyDto)
        {
            await _replyService.AddPostReplyAsnyc(replyDto);
            return Ok(new { message = "Reply added successfully" });
        }
    }
}
