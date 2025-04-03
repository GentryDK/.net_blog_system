using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Abp.AspNet.JwpBearer
{
    public class TokenCreateModel
    {
        private readonly IConfiguration _configuration;

        public TokenCreateModel(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public TokenCreateModel()
        {

        }

        public string userId { get; set; }
        public string securityKey { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
        public TimeSpan ts { get; set; }

        public string GetToken(string customerNo, params Claim[] claims)
        {
            //从配置文件中获取token配置,并赋值到TokenManagement中
            TokenCreateModel tokenCreateModel = new TokenCreateModel
            {
                audience = _configuration.GetValue<string>("JwtAuth:Audience"),
                issuer = _configuration.GetValue<string>("JwtAuth:Issuer"),
                securityKey = _configuration.GetValue<string>("JwtAuth:SecurityKey"),
                userId = customerNo,//用户id可以从数据库中获取
                ts = TimeSpan.FromHours(2),
            };
            return JwtCreator.CreateToken(tokenCreateModel, claims);
        }
    }
}
