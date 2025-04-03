using Microsoft.Extensions.DependencyInjection;
using MyBlog.UserSystem;
using MyBlog.UserSystem.EntityFrameworkCore;

IServiceCollection services = new ServiceCollection();
services.AddApplication<MyBlogUserSystemWebModule>();
var serviceProvider = services.BuildServiceProvider();
var dbContext = serviceProvider.GetRequiredService <MyBlogUserSystemDbContext>();

dbContext.Migrator();
