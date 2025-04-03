using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Abp.AspNet.JwpBearer;
using MyBlog.BlogView.Application;
using Microsoft.OpenApi.Models;
using MyBlog.BlogSystem.Domain.AppSettings;
using Volo.Abp;
using MyBlog.BlogView.HttpApi.Client;
using MyBlog.BlogSystem.web.filters;

namespace MyBlog.BlogView.web
{
    [DependsOn(
   typeof(AbpAspNetCoreMvcModule),
   typeof(AbpAspNetBearerModule),
   typeof(BlogViewHttpClientModule),
   typeof(MyBlogViewApplicationModule),
   typeof(AbpAutofacModule)
          )]
    public class MyBlogViewWebModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //会给所有方法上都添加这个filter过滤器
            context.Services.AddControllers(c => c.Filters.Add<CurrentUserAuthorizationFilterAttribute>());

            var configuration = context.Services.GetConfiguration();

            //Configure是abp中获取appsetting模块的功能类,获取后通过IOptions<UploadSettings>去注入
            Configure<UploadSettings>(configuration.GetSection("UploadSettings"));

            var origins = configuration.GetSection("AllowOrigins").Get<string[]>();
            //允许头部请求、请求方式(get、post）、允许域名.AllowAnyOrigin()  .WithOrigins()只能使用固定的域名
            context.Services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyMethod().AllowAnyHeader().WithOrigins(origins).AllowCredentials()));
            context.Services.AddEndpointsApiExplorer();
            context.Services.AddSwaggerGen(c =>
            {
                //配置 Swagger 文档生成器时使用的一个函数。这个函数决定了哪些 API 应该被包含在生成的文档中。
                //在这个例子中，函数总是返回 true，这意味着所有的 API 都会被包含在文档中。
                c.DocInclusionPredicate((docName, dscription) => true);

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserSystem", Version = "v1" });
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() { ...});：这是在定义一个安全方案。
                //这个方案的类型是 “ApiKey”，位于请求头（Header）中，名称是 “Authorization”，格式是 “JWT”，
                //方案是 “Bearer”。这通常用于在 Swagger UI 中添加一个可用于发送带有 JWT 的请求的输入框。
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "xxxxxxxx",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                //这是在添加一个安全需求，它引用了上面定义的 “Bearer” 安全方案。
                //这意味着所有的 API 都需要这个安全方案。
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseCors();
        }
    }
}
