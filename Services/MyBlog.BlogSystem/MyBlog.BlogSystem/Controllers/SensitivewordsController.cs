using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBlog.BlogSystem.Application.Contract.SensitiveApp;
using MyBlog.BlogSystem.web.filters;

namespace MyBlog.BlogSystem.web.Controllers
{
    [Route("MyBlog/[controller]")]
    [ApiController]
    public class SensitivewordsController : ControllerBase
    {
       private ISensitiveService _sensitiveService { get; set; }

        public SensitivewordsController(ISensitiveService sensitiveService) 
        {
            this._sensitiveService = sensitiveService;
        }

        [HttpPost("UploadSentitiveWords")]
        [Authorize]
        [PermissionFilter(superAdminOnly: true)]
        public async Task UploadSensitiveWords(IFormFile sensitiveFile)
        {
           await _sensitiveService.SaveFile(sensitiveFile);
        }
    }
}
