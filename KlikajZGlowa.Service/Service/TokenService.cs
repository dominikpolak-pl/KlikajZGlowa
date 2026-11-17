using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KlikajZGlowa.Service.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _config = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
            _userManager = userManager;
        }

        public async Task<string> CreateAccessTokenAsync(ApplicationUser user_in)
        {
            var user= user_in;
     
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var expiresMinutes = double.Parse(_config["Jwt:ExpireMinutes"] ?? "60");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        public string CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}
