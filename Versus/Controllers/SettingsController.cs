using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Core.EF;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly VersusContext _context;

        public SettingsController(VersusContext context)
        {
            _context = context;
        }

        // GET: api/Settings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Settings>>> GetSettings()
        {
            return await _context.Settings
                .Include(s => s.Notifications)
                .ToListAsync();
        }

        // GET: api/Settings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Settings>> GetSettings(Guid id)
        {
            var settings = await _context.Settings
                .Include(s => s.Notifications)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (settings == null)
            {
                return NotFound();
            }

            return settings;
        }

        // PUT: api/Settings/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSettings(Guid id, Settings settings)
        {
            settings.Id = id;

            _context.Entry(settings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingsExists(id))
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

        // POST: api/Settings
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Settings>> PostSettings(Settings settings)
        {
            if(settings.Notifications == null)
                settings.Notifications = new Notifications();
            _context.Settings.Add(settings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSettings", new { id = settings.Id }, settings);
        }

        // DELETE: api/Settings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Settings>> DeleteSettings(Guid id)
        {
            var settings = await _context.Settings.FindAsync(id);
            if (settings == null)
            {
                return NotFound();
            }

            _context.Settings.Remove(settings);
            await _context.SaveChangesAsync();

            return settings;
        }

        private bool SettingsExists(Guid id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}
