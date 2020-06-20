using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Data.Converters;
using Versus.Core.EF;
using Versus.Data.Entities;
using Versus.Data.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpGet("full/{id}")]
        public async Task<ActionResult<UserDto>> GetUserWithData(Guid id)
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
                .FirstOrDefaultAsync(u => u.Id == id));

            user.Settings.User = null;
            user.Vip.User = null;
            user.Exercises.User = null;

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

        [HttpGet("champions")]
        public async Task<ActionResult<Champion>> GetChampions()
        {
            string sql = "select u.\"UserName\" as UserName, " +
                         "abs.\"Wins\" + sqs.\"Wins\" + pls.\"Wins\" + phs.\"Wins\" as Wins, " +
                         "u.\"Country\" as Country, " +
                         "0 as Rate " +
                         "from public.\"AspNetUsers\" as u " +
                         "join public.\"Exercises\" as ex on ex.\"UserId\" = u.\"Id\" " +
                         "join public.\"Exercise\" as abs on abs.\"Id\" = ex.\"AbsId\" " +
                         "join public.\"Exercise\" as sqs on sqs.\"Id\" = ex.\"SquatsId\" " +
                         "join public.\"Exercise\" as pls on pls.\"Id\" = ex.\"PullUpsId\" " +
                         "join public.\"Exercise\" as phs on phs.\"Id\" = ex.\"PushUpsId\" ";
                         
            string sqlWhere = "where u.\"IsVip\" = true ";
            string sqlLimit = "order by Wins desc limit 7; ";
            var champs = _context.Champions.FromSqlRaw(sql + sqlWhere + sqlLimit).ToList();
            for(var i = 0; i < champs.Count; i++)
                champs[i].Rate = i + 1;
            var userName = User.Identity.Name;
            if (userName == null || champs.Any(c => c.UserName == userName))
                return Ok(champs);
            var user = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(u => u.Abs)
                .Include(u => u.Exercises)
                .ThenInclude(u => u.Squats)
                .Include(u => u.Exercises)
                .ThenInclude(u => u.PullUps)
                .Include(u => u.Exercises)
                .ThenInclude(u => u.PushUps)
                .FirstOrDefaultAsync(u => u.UserName == userName);
            var champsAll = _context.Champions.FromSqlRaw(sql + "order by Wins desc;").ToList();
            int rate = -1;
            for (var i = 7; i < champsAll.Count; i++)
            {
                if (champsAll[i].UserName == userName)
                    rate = i + 1;
            }
            champs.Add(new Champion
            {
                UserName = userName,
                Wins = user.Exercises.Abs.Wins + user.Exercises.Squats.Wins +
                                             user.Exercises.PullUps.Wins + user.Exercises.PushUps.Wins,
                Rate = rate,
                Country = user.Country
            });
            return Ok(champs);



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

            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Пользователя с таким ID не существует");

            var reqUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            reqUser.Country = user.Country;
            reqUser.UserName = user.UserName;
            reqUser.Email = user.Email;
            reqUser.Token = user.Token;
            reqUser.LastTime = user.LastTime;
            reqUser.Online = user.Online;
            
            await _userManager.UpdateAsync(reqUser);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(reqUser);
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
        }

        
        [HttpPut("token/{token}")]
        public async Task<IActionResult> PutUser(string token)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            user.Token = token;
            
            await _userManager.UpdateAsync(user);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
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
