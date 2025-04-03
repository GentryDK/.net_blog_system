
using Microsoft.AspNetCore.Authentication.Cookies;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotGateWay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var cookieSettings = builder.Configuration.GetSection("CookieSettings");

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Domain = cookieSettings["Domain"];   
                options.Cookie.SameSite = Enum.Parse<SameSiteMode>(cookieSettings["SameSite"]); 
                options.Cookie.SecurePolicy = Enum.Parse<CookieSecurePolicy>(cookieSettings["SecurePolicy"]);
            });

            builder.Services.AddOcelot();
            builder.Services.AddControllers();

            var configration = builder.Configuration;
            var origins = configration.GetSection("AllowOrigins").Get<string[]>();
            builder.Services.AddCors(c => 
            c.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins(origins).AllowCredentials()));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseAuthorization();
            app.MapControllers();
            app.UseOcelot().Wait();
            app.Run();
        }
    }
}
