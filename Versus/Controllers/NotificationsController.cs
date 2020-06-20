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
    public class NotificationsController : ControllerBase
    {
        private readonly VersusContext _context;
        private readonly UserManager<User> _userManager;

        public NotificationsController(VersusContext context, UserManager<User> um)
        {
            _context = context;
            _userManager = um;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notifications>>> GetNotifications()
        {
            return await _context.Notifications.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notifications>> GetNotifications(Guid id)
        {
            var notifications = await _context.Notifications.FindAsync(id);

            if (notifications == null)
            {
                return NotFound();
            }

            return notifications;
        }
        
        [HttpGet("user/{id}")]
        public async Task<ActionResult<Settings>> GetNotificationsByUserId(Guid id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Не найден пользователь с таким Id");

            var user = await _userManager.Users
                .Include(u => u.Settings)
                .ThenInclude(s => s.Notifications)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user.Settings.Notifications == null)
            {
                return NotFound("У Settings отсутствует связанная сущность Notifications");
            }

            return Ok(user.Settings.Notifications);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotifications(Guid id, Notifications notifications)
        {
            notifications.Id = id;

            _context.Entry(notifications).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationsExists(id))
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
        
        [HttpPut("user/{id}/{param}/{value}")]
        public async Task<IActionResult> PutNotificationsByUserId(Guid id, string param, bool value)
        {
            if (!await _userManager.Users.AnyAsync(s => s.Id == id))
                return NotFound("Такого UserID не существует");

            var reqUser = await _userManager.Users
                .Include(u => u.Settings)
                .ThenInclude(s => s.Notifications)
                .FirstOrDefaultAsync(s => s.Id == id);

            var reqNotifications = reqUser.Settings.Notifications;
    
            if(param == "mon")
                reqNotifications.Mon = value;
            else if(param == "tue")
                reqNotifications.Tue = value;
            else if(param == "wed")
                reqNotifications.Wed = value;
            else if(param == "thu")
                reqNotifications.Thu = value;
            else if(param == "fri")
                reqNotifications.Fri = value;
            else if(param == "sat")
                reqNotifications.Sat = value;
            else 
                reqNotifications.Sun = value;

            _context.Entry(reqNotifications).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(reqNotifications);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationsExists(reqUser.Settings.NotificationsId))
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
        public async Task<ActionResult<Notifications>> PostNotifications(Notifications notifications)
        {
            _context.Notifications.Add(notifications);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNotifications", new { id = notifications.Id }, notifications);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Notifications>> DeleteNotifications(Guid id)
        {
            var notifications = await _context.Notifications.FindAsync(id);
            if (notifications == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notifications);
            await _context.SaveChangesAsync();

            return notifications;
        }

        private bool NotificationsExists(Guid id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
