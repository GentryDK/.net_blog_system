using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto
{
    public class AddPostTypeDto
    {
        public string? PostTypeId { get; set; }

        public string PostTypeName {  get; set; }

        public string? Cover {  get; set; }

        public string? TypeBrief { get; set; }

        public IFormFile? CoverFile {  get; set; }
    }
}
