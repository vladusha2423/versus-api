using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
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

        public async Task<Myself> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                var appUser = await _userManager.FindByNameAsync(username);
                return new Myself
                {
                    Jwt = await _jwt.GenerateJwt(appUser),
                    UserId = appUser.Id
                };
            }
            return null;
        }

        public async Task<object> Logout()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        public async Task<Myself> Register(UserDto item)
        {
            User user = UserConverter.Convert(item);
            var result = await _userManager.CreateAsync(user, item.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                await _userManager.AddToRoleAsync(user, "user");
                return new Myself
                {
                    Jwt = await _jwt.GenerateJwt(user),
                    UserId = user.Id
                };
                    
            }

            return null;
        }
    }



}
