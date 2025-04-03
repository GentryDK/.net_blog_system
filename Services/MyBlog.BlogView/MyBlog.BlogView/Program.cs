
using MyBlog.BlogView.web;

namespace MyBlog.BlogView
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Host.UseAutofac();
            builder.Services.AddApplication<MyBlogViewWebModule>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.InitializeApplication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
