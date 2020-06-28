using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Core.EF;
using Versus.Data.Entities;
using Versus.Messaging.Interfaces;
using Versus.WebSockets;

namespace Versus.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMobileMessagingClient _mmc;
        private readonly UserManager<User> _userManager;
        private readonly MessagesHandler _messagesHandler;
        private readonly VersusContext _context;


        public MessagesController(
            IMobileMessagingClient mmc, 
            UserManager<User> um, 
            VersusContext vc,
            MessagesHandler messagesHandler)
        {
            _mmc = mmc;
            _userManager = um;
            _messagesHandler = messagesHandler;
            _context = vc;
        }

        [HttpPost("ws/all")]
        public async Task<ActionResult<object>> SendWsMessageToAll([FromBody] string text)
        {
            await _messagesHandler.SendMessageToAllAsync(text);
            return Ok();
        } 

        [HttpPost("ws/user/{userId}")]
        public async Task<ActionResult<object>> SendWsMessageToUser(Guid userId, [FromBody] string text)
        {
            var userSocket = await _context.UserSockets
                .FirstOrDefaultAsync(us => us.UserId == userId);

            await _messagesHandler.SendMessageAsync(userSocket.SocketId, text);
            return Ok();
        } 

        [HttpPost("notifications/user/{userId}")]
        public async Task<ActionResult<object>> SendMessageByUserId(Guid userId, Notification notification)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == userId))
                return NotFound("Пользователь с таким ID отсутствует");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user.Token == null)
                return BadRequest("У пользователя отсутствует FCM-токен для уведомлений");

            var result = await _mmc.SendNotification(user.Token, notification.Title, notification.Body);
            return Ok(result);
        }

        [HttpPost("notifications/token/{token}")]
        public async Task<ActionResult<object>> SendMessageByToken(string token, Notification notification)
        {
            if (token == null)
                return BadRequest("Отсутствует FCM-токен для отправки уведомления");

            if (notification.Body == null || notification.Title == null)
                return BadRequest("Недостаточно данных");

            var result = await _mmc.SendAndroidNotification(token, notification.Title, notification.Body, new Dictionary<string, string>
            {
                {"Type", "Invite"},
            });
            return Ok(result);
        }

        [HttpPost("data/user/{userId}")]
        public async Task<ActionResult<object>> SendDataByUserId(Guid userId, Notification notification)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == userId))
                return NotFound("Пользователь с таким ID отсутствует");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user.Token == null)
                return BadRequest("У пользователя отсутствует FCM-токен для уведомлений");

            var result = await _mmc.SendNotification(user.Token, notification.Title, notification.Body);
            return Ok(result);
        }

        [HttpPost("data/token/{token}")]
        public async Task<ActionResult<object>> SendDataByToken(string token, Notification notification)
        {
            if (token == null)
                return BadRequest("Отсутствует FCM-токен для отправки уведомления");

            if (notification.Body == null || notification.Title == null)
                return BadRequest("Недостаточно данных");

            var result = await _mmc.SendNotification(token, notification.Title, notification.Body);
            return Ok(result);
        }
        
    }
}