using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Versus.Auth.Interfaces;
using Versus.Core.EF;
using Versus.Data.Dto;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Authorize(Roles = "admin")]
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

        [HttpPost]
        [AllowAnonymous]
        [Produces(typeof(object))]
        public async Task<ActionResult<object>> Login([FromBody] Login form)
        {
            try
            {
                var result = await _auth.Login(form.UserName, form.Password);
                if (result == null)
                    return BadRequest();
                return result;
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
                var result = await _auth.Register(item);
                if (result == null)
                    return BadRequest();

                var user = await _userManager.FindByNameAsync(item.UserName);

                var exercises = new Exercises
                {
                    UserId = user.Id
                };

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

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
    }

}
