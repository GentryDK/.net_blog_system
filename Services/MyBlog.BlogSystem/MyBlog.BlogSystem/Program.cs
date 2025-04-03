using MyBlog.BlogSystem.web;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAutofac();
builder.Services.AddApplication<MyBlogSystemWebModule>();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.InitializeApplication();
app.UseAuthorization();
app.MapControllers();

app.Run();
