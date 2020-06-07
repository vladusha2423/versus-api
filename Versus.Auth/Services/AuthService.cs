using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versus.Auth.Interfaces;
using Versus.Data.Converters;
using Versus.Data.Dto;
using Versus.Data.Entities;

namespace Versus.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtGenerator _jwt;

        public AuthService(SignInManager<User> sim, UserManager<User> um, IJwtGenerator jwt)
        {
            _signInManager = sim;
            _userManager = um;
            _jwt = jwt;
        }

        public async Task<object> Login(string token)
        {
            if (token == null)
                return null;

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Token == token);

            if (user == null) return null;
            
            await _signInManager.SignInAsync(user, true);
                            
            return await _jwt.GenerateJwt(user);

        }

        
        public async Task<object> Register(UserDto item)
        {
            User user = UserConverter.Convert(item);
            var result = await _userManager.CreateAsync(user, item.Token);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, true);
                await _userManager.AddToRoleAsync(user, "user");
                return await _jwt.GenerateJwt(user);
            }

            return null;
        }
    }



}
