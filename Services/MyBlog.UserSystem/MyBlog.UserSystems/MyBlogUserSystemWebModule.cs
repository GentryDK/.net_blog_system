using Abp.AspNet.JwpBearer;
using Microsoft.OpenApi.Models;
using MyBlog.UserSystem.Application;
using MyBlog.UserSystem.Domain.AppSettings;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MyBlog.UserSystem
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAspNetBearerModule),
        typeof(MyBlogUserSystemApplicationMoudule),
        typeof(AbpAutofacModule)
               )]
    public class MyBlogUserSystemWebModule:AbpModule
    {
        /// <summary>
        /// 这里面是写服务的注册的
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddControllers();

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

            //自动api控制，将一个类中所有方法转换成api的接口,这样这个类就成了这个微服务的一个接口
            //被转换的类需要继承自IRemoteService(这个在ApplicationService这个基类中是有继承的)
            Configure<AbpAspNetCoreMvcOptions>(options => {
                options.ConventionalControllers.Create(typeof(MyBlogUserSystemApplicationMoudule).Assembly);
            });
        }

        /// <summary>
        /// 写入需要使用的中间键的(写在这里会自动帮我们添加到Program中)
        /// </summary>
        /// <param name="context"></param>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();
            app.UseAuthentication();
            app.UseStaticFiles();
            //app.UseHttpsRedirection();
            app.UseAuthorization();
        }
    }
}
