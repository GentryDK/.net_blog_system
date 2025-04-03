using MyBlog.BlogSystem.Domain.PostInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application.Contract.PostApp.Dto
{
    public class PostServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Post Post { get; set; }
    }
}
