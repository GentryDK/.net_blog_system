using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Abp.AspNet.JwpBearer
{
    public class JwtCreator
    {
       public static string CreateToken(TokenCreateModel tokenCreateModel,params Claim[] claims)
        {
            var claimList = claims?.ToList() ?? new List<Claim>();
            claimList.Add(new Claim("userId", tokenCreateModel.userId.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenCreateModel.securityKey));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: tokenCreateModel.issuer,
                audience: tokenCreateModel.audience,
                claims: claimList.ToArray(),
                expires: DateTime.Now.Add(tokenCreateModel.ts),
                signingCredentials: creds
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }
    }
}
