using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Core.EF;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VipsController : ControllerBase
    {
        private readonly VersusContext _context;

        public VipsController(VersusContext context)
        {
            _context = context;
        }

        // GET: api/Vips
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VIP>>> GetVip()
        {
            return await _context.Vip.ToListAsync();
        }

        // GET: api/Vips/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VIP>> GetVIP(Guid id)
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

        // POST: api/Vips
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<VIP>> PostVIP(VIP vIP)
        {
            _context.Vip.Add(vIP);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVIP", new { id = vIP.Id }, vIP);
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
