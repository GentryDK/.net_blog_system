using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application.Contract.ReplyApp.Dto
{
    public class ReplyInfoDto
    {
        public List<ReplyDto> ReplyDtos { get; set; }

        public int PostReplyCount {  get; set; }
    }
}
