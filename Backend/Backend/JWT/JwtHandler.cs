using Backend;
using Domain;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;

namespace ASPLAB2.API.JWT
{
    public class JwtHandler
    {
        private readonly AppSettings _appSettings;

        public JwtHandler(AppSettings appSettings)
        {
            this._appSettings = appSettings;
        }


        public string MakeToken(User user)
        {
            Guid tokenGuid = Guid.NewGuid();

            string tokenId = tokenGuid.ToString();

            var claims = new List<Claim>
            {
                 
                 new Claim(JwtRegisteredClaimNames.Iss, _appSettings.JwtSettings.Issuer, ClaimValueTypes.String), // Iss je issuer
                 new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // iat je issuedAt
                 new Claim("Username", user.Username),
                 new Claim("Email", user.Email),
                 new Claim("Id", user.Id.ToString()),
            
            };

        
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSettings.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _appSettings.JwtSettings.Issuer, 
                audience: "Any", 
                claims: claims,
                notBefore: now,
                expires: now.AddSeconds(_appSettings.JwtSettings.DurationSeconds),
                signingCredentials: credentials);

         
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
