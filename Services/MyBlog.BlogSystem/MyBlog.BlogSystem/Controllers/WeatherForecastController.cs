using Microsoft.AspNetCore.Mvc;
using MyBlog.BlogSystem.Application.Contract.SensitiveApp;
using MyBlog.UserSystem.Application.Contract.RoleApp;

namespace MyBlog.BlogSystem.Controllers
{
    [ApiController]
    [Route("MyBlog/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ISensitiveService _sensitiveService;
       private readonly ILogger<WeatherForecastController> _logger;
        private IRoleService _roleService;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IRoleService roleService,
            ISensitiveService sensitiveService
            )
        {
            _logger = logger;
           _roleService = roleService;
            _sensitiveService = sensitiveService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<string> Get()
        {
           await _sensitiveService.GetWordsInCacheAsync();
            return "测试成功";
        }
    }
}
