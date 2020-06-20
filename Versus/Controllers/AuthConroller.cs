using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versus.Auth.Interfaces;
using Versus.Core.EF;
using Versus.Data.Dto;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Authorize]
    [Route("api/[action]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        private readonly VersusContext _context;
        private readonly UserManager<User> _userManager;


        public AuthController(IAuthService auth, VersusContext vc, UserManager<User> um)
        {
            _auth = auth;
            _context = vc;
            _userManager = um;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<bool> Myself()
        {
            var sender = User.Identity.IsAuthenticated;
            return Ok(sender);
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces(typeof(object))]
        public async Task<ActionResult<object>> Login([FromBody] Login form)
        {
            try
            {
                if (form.UserName == null || form.Password == null)
                    return BadRequest("Недостаточно данных");
                if (!await _userManager.Users.AnyAsync(u => u.UserName == form.UserName))
                    return NotFound("Пользователя с таким именем не существует");

                var user = await _userManager.FindByNameAsync(form.UserName);
                user.Online = true;
                
                if (form.Token != null)
                    user.Token = form.Token;
                await _userManager.UpdateAsync(user);
                
                var result = await _auth.Login(form.UserName, form.Password);
                if (result == null)
                    return Unauthorized("Неверный пароль");
                return Ok(new MyselfInfo
                {
                    Country = user.Country,
                    Email = user.Email,
                    Jwt = result.Jwt,
                    UserId = result.UserId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        [Produces(typeof(object))]
        public async Task<ActionResult<object>> Logout()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Unauthorized("Вы не авторизованы");
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                user.Online = false;
                await _auth.Logout();
                return Ok("Вы успешно вышли из системы");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces(typeof(object))]
        public async Task<ActionResult<object>> Register([FromBody] UserDto item)
        {
            try
            {
                if (item.UserName == null || item.Email == null)
                    return StatusCode(400, "Недостаточно данных");
                if (await _userManager.Users.AnyAsync(u => u.UserName == item.UserName))
                {
                    var userAuth = await _userManager.FindByNameAsync(item.UserName);
                    userAuth.Online = true;
                    userAuth.Country = item.Country;
                    await _userManager.UpdateAsync(userAuth);
                    
                    if (item.Email != userAuth.Email)
                        return StatusCode(409, "Неверный Email для " + item.UserName);
                    var resultAuth = await _auth.Login(item.UserName, item.Password);
                    if (resultAuth == null)
                        return Unauthorized("Неверный пароль");
                    
                    
                    return Ok(resultAuth);
                }
                    
                if (await _userManager.Users.AnyAsync(u => u.Email == item.Email))
                    return StatusCode(418, "Пользователь с таким Email уже существует");
                var result = await _auth.Register(item);
                if (result == null)
                    return BadRequest("Некорректные данные");

                var user = await _userManager.FindByNameAsync(item.UserName);

                var exercises = new Exercises{UserId = user.Id};

                if (exercises.PushUps == null)
                    exercises.PushUps = new Exercise();
                if (exercises.PullUps == null)
                    exercises.PullUps = new Exercise();
                if (exercises.Abs == null)
                    exercises.Abs = new Exercise();
                if (exercises.Squats == null)
                    exercises.Squats = new Exercise();
                _context.Exercises.Add(exercises);

                var vip = new VIP
                {
                    UserId = user.Id
                };
                _context.Vip.Add(vip);

                var settings = new Settings
                {
                    UserId = user.Id
                };
                if (settings.Notifications == null)
                    settings.Notifications = new Notifications();
                _context.Settings.Add(settings);

                await _context.SaveChangesAsync();

                return StatusCode(201, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }

        [HttpPost]
        [Produces(typeof(object))]
        public async Task<ActionResult<object>> Offline()
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                user.Online = false;
                var result = await _userManager.UpdateAsync(user);
                return Ok(result);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Produces(typeof(object))]
        public async Task<ActionResult<object>> Online()
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                user.Online = true;
                var result = await _userManager.UpdateAsync(user);
                return Ok(result);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }

}
