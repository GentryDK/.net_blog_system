using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBlog.BlogSystem.Domain.PostInfo;

namespace MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto
{
    public class PostTypeServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PostType PostType { get; set; }
    }
}
