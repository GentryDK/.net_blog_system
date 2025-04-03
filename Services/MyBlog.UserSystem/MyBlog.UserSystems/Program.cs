using MyBlog.UserSystem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//替换原生容器
builder.Host.UseAutofac();
//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApplication<MyBlogUserSystemWebModule>();

//这个方法是ASP.NET Core中的一个扩展方法，用于注册服务以暴露API端点的信息。
//它主要用于生成API文档，例如Swagger12。通过调用这个方法，API文档工具可以获取有关端点的详细信息，
//包括URL、HTTP方法、请求参数和响应模式等。
//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

//这个方法通常用于初始化应用程序的配置和服务。
//它可以包括设置依赖注入容器、配置中间件、注册服务等。
//具体的实现可能因项目而异，但总体目标是确保应用程序在启动时已正确配置。
app.InitializeApplication();

app.MapControllers();

app.Run();
