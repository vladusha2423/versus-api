using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Core.EF;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly VersusContext _context;
        private readonly UserManager<User> _userManager;

        public SettingsController(VersusContext context, UserManager<User> um)
        {
            _context = context;
            _userManager = um;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Settings>>> GetSettings()
        {
            return await _context.Settings
                .Include(s => s.Notifications)
                .ToListAsync();
        }

        
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
        
        [HttpGet("user/{id}")]
        public async Task<ActionResult<Settings>> GetSettingsByUserId(Guid id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Не найден пользователь с таким Id");

            var user = await _userManager.Users
                .Include(u => u.Settings)
                .ThenInclude(s => s.Notifications)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user.Settings == null)
            {
                return NotFound("У пользователя отсутствует связанная сущность \"Settings\"");
            }

            user.Settings.User = null;

            return Ok(user.Settings);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSettings(Guid id, Settings settings)
        {
            if (!await _context.Settings.AnyAsync(s => s.Id == id))
                return NotFound("Такого SettingsID не существует");

            var reqSettings = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);

            reqSettings.Invites = settings.Invites;
            reqSettings.Language = settings.Language;
            reqSettings.Sound = settings.Sound;
            reqSettings.IsNotifications = settings.IsNotifications;

            _context.Entry(reqSettings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(reqSettings);
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
        }
        
        [HttpPut("user/{id}/{param}/{value}")]
        public async Task<IActionResult> PutSettingsByUserId(Guid id, string param, bool value)
        {
            if (!await _userManager.Users.AnyAsync(s => s.Id == id))
                return NotFound("Такого UserID не существует");

            var reqUser = await _userManager.Users
                .Include(u => u.Settings)
                .ThenInclude(s => s.Notifications)
                .FirstOrDefaultAsync(s => s.Id == id);

            var reqSettings = reqUser.Settings;

            if(param == "invites")
                reqSettings.Invites = value;
            else if (param == "language")
                reqSettings.Language = value;
            else if (param == "sound")
                reqSettings.Sound = value;
            else
                reqSettings.IsNotifications = value;

            _context.Entry(reqSettings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                reqSettings.User = null;
                return Ok(reqSettings);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingsExists(reqUser.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<Settings>> PostSettings(Settings settings)
        {
            if(settings.Notifications == null)
                settings.Notifications = new Notifications();
            _context.Settings.Add(settings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSettings", new { id = settings.Id }, settings);
        }
        
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
