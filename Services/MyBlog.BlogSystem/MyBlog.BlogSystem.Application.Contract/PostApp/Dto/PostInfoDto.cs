using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application.Contract.PostApp.Dto
{
    public class PostInfoDto
    {
        public List<PostDto> PostListDto { get; set; }

        public int PostCount { get; set; }
    }
}
