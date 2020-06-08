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

        public async Task<object> Login(string username, string password)
        {
            if (username == null || password == null)
                return null;

            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                var appUser = await _userManager.FindByNameAsync(username);
                return await _jwt.GenerateJwt(appUser);
            }
            return null;
        }


        public async Task<object> Register(UserDto item)
        {
            User user = UserConverter.Convert(item);
            var result = await _userManager.CreateAsync(user, item.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                await _userManager.AddToRoleAsync(user, "user");
                return await _jwt.GenerateJwt(user);
            }

            return null;
        }
    }



}
