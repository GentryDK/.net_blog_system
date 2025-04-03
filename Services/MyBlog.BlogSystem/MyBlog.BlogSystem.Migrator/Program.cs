using Microsoft.Extensions.DependencyInjection;
using MyBlog.BlogSystem.web;
using MyBlog.BlogSystem.EntityFrameworkCore;

IServiceCollection services = new ServiceCollection();
services.AddApplication<MyBlogSystemWebModule>();
var serviceProvider = services.BuildServiceProvider();
var dbContext = serviceProvider.GetRequiredService<MyBlogSystemDbContext>();

dbContext.Migrator();


