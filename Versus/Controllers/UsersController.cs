using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Data.Converters;
using Versus.Core.EF;
using Versus.Data.Entities;
using Versus.Data.Dto;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly VersusContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(VersusContext context, UserManager<User> um)
        {
            _context = context;
            _userManager = um;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUser()
        {
            return UserConverter.Convert(await _userManager.Users.ToListAsync());
        }

        // GET: api/Users
        [HttpGet("full")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUserWithData()
        {
            return UserConverter.Convert(await _userManager.Users
                .Include(u => u.Settings)
                .ThenInclude(s => s.Notifications)
                .Include(u => u.Vip)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PullUps)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PushUps)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Abs)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Squats)
                .ToListAsync());
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = UserConverter.Convert(
                await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/5
        [HttpGet("token/{token}")]
        public async Task<ActionResult<UserDto>> GetUserByToken(string token)
        {
            var user = UserConverter.Convert(
                await _userManager.Users.FirstOrDefaultAsync(u => u.Token == token));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/5
        [HttpGet("full/token/{token}")]
        public async Task<ActionResult<UserDto>> GetUserWithData(string token)
        {
            var user = UserConverter.Convert(await _userManager.Users
                .Include(u => u.Settings)
                .ThenInclude(s => s.Notifications)
                .Include(u => u.Vip)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PullUps)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PushUps)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Abs)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Squats)
                .FirstOrDefaultAsync(u => u.Token == token));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, UserDto user)
        {
            user.Id = id;

            var result = _userManager.UpdateAsync(UserConverter.Convert(user));

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDto userDto)
        {
            var user = UserConverter.Convert(userDto);
            var result = await _userManager.CreateAsync(user);
            await _context.SaveChangesAsync();
            var newUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Token == user.Token);
            if (!result.Succeeded)
                return null;
            var r = await _userManager.AddToRoleAsync(newUser, "user");
            if (!r.Succeeded)
                return null;
            return CreatedAtAction("GetUser", new { id = newUser.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(Guid id)
        {
            return _userManager.Users.Any(e => e.Id == id);
        }
    }
}
