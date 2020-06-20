using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Core.EF;
using Versus.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VipsController : ControllerBase
    {
        private readonly VersusContext _context;
        private readonly UserManager<User> _userManager;
        

        public VipsController(VersusContext context, UserManager<User> um)
        {
            _context = context;
            _userManager = um;
        }

        // GET: api/Vips
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VIP>>> GetVip()
        {
            return await _context.Vip.ToListAsync();
        }

        // GET: api/Vips/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VIP>> GetVip(Guid id)
        {
            var vIP = await _context.Vip.FindAsync(id);

            if (vIP == null)
            {
                return NotFound();
            }

            return vIP;
        }

        // PUT: api/Vips/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVIP(Guid id, VIP vIP)
        {
            vIP.Id = id;

            _context.Entry(vIP).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VIPExists(id))
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

        // PUT: api/Vips/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("user/{id}")]
        public async Task<IActionResult> PutVIPByUserId(Guid id, VIP vIP)
        {
            if (!await _userManager.Users
                .AnyAsync(u => u.Id == id))
                return NotFound("Пользователя с таким Id  не существует");

            var user = await _userManager.Users
                .Include(u => u.Vip)
                .FirstOrDefaultAsync(u => u.Id == id);

            user.Vip.Begin = vIP.Begin;
            user.Vip.Duration = vIP.Duration;

            _context.Entry(user.Vip).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(user.Vip);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VIPExists(user.Vip.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Vips
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<VIP>> PostVIP(VIP vIP)
        {
            _context.Vip.Add(vIP);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVip", new { id = vIP.Id }, vIP);
        }

        
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<VIP>> PostVIPToUser(Guid userId, VIP vIP)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == userId))
                return NotFound("Пользователя с таким ID не существует");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (await _context.Vip.AnyAsync(v => v.UserId == userId))
            {
                var vip = await _context.Vip.FirstOrDefaultAsync(v => v.UserId == userId);
                vip.Begin = vIP.Begin;
                vip.Duration = vIP.Duration;
                
                user.IsVip = true;
                await _userManager.UpdateAsync(user);
                _context.Entry(vip).State = EntityState.Modified;
                vip.User = null;
                return Ok(vip);
            }

            vIP.UserId = userId;
            _context.Vip.Add(vIP);
            user.IsVip = true;
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            vIP.User = null;
            
            return StatusCode(201, vIP);
        }

        // DELETE: api/Vips/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<VIP>> DeleteVIP(Guid id)
        {
            var vIP = await _context.Vip.FindAsync(id);
            if (vIP == null)
            {
                return NotFound();
            }

            _context.Vip.Remove(vIP);
            await _context.SaveChangesAsync();

            return vIP;
        }

        private bool VIPExists(Guid id)
        {
            return _context.Vip.Any(e => e.Id == id);
        }
    }
}
