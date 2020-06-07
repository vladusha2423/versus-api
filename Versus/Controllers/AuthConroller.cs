using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Versus.Auth.Interfaces;
using Versus.Data.Dto;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[action]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;


        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces(typeof(object))]
        public async Task<ActionResult<object>> Login([FromBody] FcmToken form)
        {
            try
            {
                var result = await _auth.Login(form.Token);
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
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
    }

}
