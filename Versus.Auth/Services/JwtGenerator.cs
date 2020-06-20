using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Versus.Auth.Interfaces;
using Versus.Data.Entities;

namespace Versus.Auth.Services
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public JwtGenerator(UserManager<User> um,
            IConfiguration conf)
        {
            _userManager = um;
            _configuration = conf;
        }

        public async Task<object> GenerateJwt(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserName", user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");


            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Key").ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(30));

            var token = new JwtSecurityToken(
                _configuration.GetSection("Issuer").ToString(),
                _configuration.GetSection("Audience").ToString(),
                claimsIdentity.Claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
    }

}
