using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Abp.AspNet.JwpBearer
{
    public class AbpAspNetBearerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            services.AddTransient<TokenCreateModel>();

            var configuration = services.GetConfiguration();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt => {
                opt.RequireHttpsMetadata = false;
                //配置 JWT 验证参数
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    //验证令牌的签发者
                    ValidateIssuer = true,
                    //验证令牌的受众
                    ValidateAudience = true,
                    ValidAudience = configuration.GetValue<string>("JwtAuth:Audience"),
                    ValidIssuer = configuration.GetValue<string>("JwtAuth:Issuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>("JwtAuth:SecurityKey"))),
                };
                opt.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        //验证失败先终止代码
                        context.HandleResponse();
                        var payload = "{\"ret\":203,\"err\":\"无登录信息或登录信息已失效，请重新登录。\"}";
                        //告诉前端返回的是json
                        context.Response.ContentType = "application/json";
                        //返回错误代码
                        context.Response.StatusCode = StatusCodes.Status203NonAuthoritative;
                        //把错误内容返回给前端
                        context.Response.WriteAsync(payload);

                        return Task.FromResult(0);
                    }
                };
            });
            base.ConfigureServices(context);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            base.OnApplicationInitialization(context);
        }
    }
}
