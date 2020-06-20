using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Versus.Core.EF;
using Versus.Data.Entities;
using Versus.WebSockets;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VersusController : Controller
    {
        private readonly VersusContext _context;
        private readonly UserManager<User> _userManager;
        private readonly MessagesHandler _messagesHandler;
        
        public VersusController(
            VersusContext context, 
            UserManager<User> um,
            MessagesHandler messagesHandler)
        {
            _context = context;
            _userManager = um;
            _messagesHandler = messagesHandler;
        }
        
        [HttpPost("find/{exercise}")]
        public async Task<ActionResult<object>> FindVersus(string exercise)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                var res = _context.Versus.Add(new Data.Entities.Versus
                {
                    DateTime = DateTime.Now,
                    InitiatorId = user.Id,
                    InitiatorName = user.UserName,
                    Exercise = exercise,
                    Status = "Searching",
                    InitiatorIterations = 0,
                    OpponentIterations = 0
                }); 
                await _context.SaveChangesAsync();
                var versusId = res.Entity.Id;

                var userName = User.Identity.Name;
                await _messagesHandler.FindJob(_context, _userManager, exercise, versusId, userName);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("reject/{userName}")]
        public async Task<ActionResult<object>> Rejection(string userName)
        {
            if (!await _userManager.Users.AnyAsync(u => u.UserName == userName))
                return NotFound("Пользователя с таким именем не существует");

            
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var versus = await _context.Versus
                    .FirstOrDefaultAsync(v => v.LastInvitedId == user.Id 
                                              && v.InitiatorName == userName);
                await _messagesHandler.FindJob(_context, _userManager, versus.Exercise, versus.Id, userName);
                return true;
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("accept/{userName}")]
        public async Task<ActionResult<object>> Acceptance(string userName)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var versus = await _context.Versus
                .FirstOrDefaultAsync(v => v.InitiatorName == userName && v.Status == "Searching");
            
            if (versus == null)
                return NotFound("Поединок отменен.");
            
            versus.Status = "Preparing";
            versus.OpponentId = user.Id;
            versus.OpponentName = user.UserName;
            _context.Entry(versus).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            try
            {
                var socketId = await UserToSocket(versus.InitiatorId);
                if (socketId != null)
                {
                    await _messagesHandler.SendMessageAsync(socketId, 
                        JsonSerializer.Serialize(
                            new Dictionary<string,string>()
                            {
                                {"Type", "Accept"},
                                {"UserName", user.UserName}
                            }));
                    return Ok();
                }
                
                // await _hubContext.Clients.User(userName)
                //     .SendAsync("Send", "Accept", user.UserName + " принял вызов.");

                return NotFound("Пользователь вышел из сети.");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("ready")]
        public async Task<ActionResult<object>> Ready()
        {
            var userName = User.Identity.Name;
            var versus = await _context.Versus.FirstOrDefaultAsync(v => 
                (v.OpponentName == userName || v.InitiatorName == userName) && 
                (v.Status == "Preparing" || v.Status == "Ready" || v.Status == "BotReady"));
            if (versus == null)
                return NotFound("Поединки для текущего пользователя отсутствуют или недействительны");
            
            if (versus.Status == "Ready")
            {
                versus.Status = "Active";
                _context.Entry(versus).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                try
                {
                    var socketInitId = await UserToSocket(versus.InitiatorId);
                    var socketOppId = await UserToSocket(versus.OpponentId);
                    if (socketInitId != null && socketOppId != null)
                    {
                        await _messagesHandler.SendMessageAsync(socketInitId, 
                            JsonSerializer.Serialize(
                                new Dictionary<string,string>()
                                {
                                    {"Type", "Start"},
                                }));
                        await _messagesHandler.SendMessageAsync(socketOppId, 
                            JsonSerializer.Serialize(
                                new Dictionary<string,string>()
                                {
                                    {"Type", "Start"},
                                }));
                        return Ok();
                    }
                    return NotFound("Пользователь вышел из сети");
                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex);
                }
            }
            else if (versus.Status == "BotReady")
            {
                versus.Status = "BotActive";
                _context.Entry(versus).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                try
                {
                    var socketInitId = await UserToSocket(versus.InitiatorId);
                    if (socketInitId != null)
                    {
                        await _messagesHandler.SendMessageAsync(socketInitId, 
                            JsonSerializer.Serialize(
                                new Dictionary<string,string>()
                                {
                                    {"Type", "Start"},
                                }));
                        return Ok();
                    }
                    return BadRequest("Ошибка отправки сообщения");
                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex);
                }
            }
            else if (versus.Status == "Preparing")
            {
                versus.Status = "Ready";
                _context.Entry(versus).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                try
                {
                    var socketId = await UserToSocket(versus.InitiatorName == userName ? 
                        versus.OpponentId : versus.InitiatorId);
                    if (socketId != null)
                    {
                        await _messagesHandler.SendMessageAsync(socketId, 
                            JsonSerializer.Serialize(
                                new Dictionary<string,string>()
                                {
                                    {"Type", "Ready"},
                                    {"UserName", userName}
                                }));
                        return Ok();
                    }
                    return NotFound("Пользователь вышел из сети");
                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex);
                }
            }
            else
            {
                return StatusCode(409, "Неподходящий статус поединка");
            }
            
        }

        [HttpPost("iteration/{num}")]
        public async Task<ActionResult<object>> Iteration(int num)
        {
            var userName = User.Identity.Name;
            var versus = await _context.Versus.FirstOrDefaultAsync(v => 
                (v.OpponentName == userName || v.InitiatorName == userName) && 
                v.Status == "Active");
            if (versus == null)
                return NotFound("Активных поединков для текущего пользователя не найдено.");

            if (versus.InitiatorName == userName)
            {
                // versus.InitiatorIterations = num;
                try
                {
                    var socketId = await UserToSocket(versus.OpponentId);
                    if (socketId == null)
                        return NotFound("Пользователь вышел из сети");
                    
                    await _messagesHandler.SendMessageAsync(socketId, 
                        JsonSerializer.Serialize(
                            new IterationResponse
                            {
                                Type = "Iteration",
                                Num = num
                            }));
                    
                    
                    // await _hubContext.Clients.User(versus.OpponentName)
                    //     .SendAsync("Send", "Iteration", num.ToString());
                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex);
                }
            }

            if (versus.OpponentName == userName)
            {
                // versus.OpponentIterations = num;
                try
                {
                    var socketId = await UserToSocket(versus.InitiatorId);
                    if (socketId == null)
                        return NotFound("Пользователь вышел из сети");
                    
                    await _messagesHandler.SendMessageAsync(socketId, 
                        JsonSerializer.Serialize(
                            new IterationResponse
                            {
                                Type = "Iteration",
                                Num = num
                            }));
                    // await _hubContext.Clients.User(versus.InitiatorName)
                    //     .SendAsync("Send", "Iteration", num.ToString());
                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex);
                }
            }
            
            // _context.Entry(versus).State = EntityState.Modified;
            // await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("completion/{num}")]
        public async Task<ActionResult<object>> Completion(int num)
        {
            // Get userName and his Versus
            var userName = User.Identity.Name;
            var versus = await _context.Versus.FirstOrDefaultAsync(v => 
                (v.OpponentName == userName || v.InitiatorName == userName) && 
                (v.Status == "Active" || v.Status == "Completion" || v.Status == "Canceled"));
            
            if (versus == null)
                return NotFound("Активного поединка для текущего пользователя не найдено");

            // If Versus is active now 
            if (versus.Status == "Active")
            {
                versus.Status = "Completion";
                if (versus.OpponentName == userName)
                    versus.OpponentIterations = num;
                else
                    versus.InitiatorIterations = num;

                _context.Entry(versus).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            // If Versus has "Completion" or "Canceled" status
            versus.Status = "Closed";
            
            //Fix Results
            if (versus.OpponentName == userName)
                versus.OpponentIterations = num;
            else
                versus.InitiatorIterations = num;
            
            //Define winner
            if (versus.OpponentIterations > versus.InitiatorIterations)
                versus.WinnerName = versus.OpponentName;
            else if (versus.InitiatorIterations > versus.OpponentIterations)
                versus.WinnerName = versus.InitiatorName;
            else
                versus.WinnerName = "Drawn Game";
            
            // Fix results
            await FixVersusResults(versus);

            _context.Entry(versus).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            try
            {
                string winner;
                winner = versus.WinnerName == "DrawnGame" ? "DrawnGame" : versus.WinnerName;
                
                //Send messages
                var socketInitId = await UserToSocket(versus.InitiatorId);
                var socketOppId = await UserToSocket(versus.OpponentId);
                if(socketInitId != null)
                    await _messagesHandler.SendMessageAsync(socketInitId, 
                        JsonSerializer.Serialize(
                            new Dictionary<string,string>()
                            {
                                {"Type", "Result"},
                                {"Winner", winner}
                            }));
                if(socketOppId != null)
                    await _messagesHandler.SendMessageAsync(socketOppId, 
                        JsonSerializer.Serialize(
                            new Dictionary<string,string>()
                            {
                                {"Type", "Result"},
                                {"Winner", winner}
                            }));
                return Ok();
                
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
            
        }

        [HttpPost("completion/{num}/{botNum}")]
        public async Task<ActionResult<object>> CompletionWithBot(int num, int botNum)
        {
            // Get userName and his Versus
            var userName = User.Identity.Name;
            var versus = await _context.Versus.FirstOrDefaultAsync(v => 
                (v.OpponentName == userName || v.InitiatorName == userName) && 
                v.Status == "BotActive");
            
            if (versus == null)
                return NotFound("Активного поединка с ботом для текущего пользователя не найдено");

            // If Versus is active now 
            if (versus.Status == "BotActive")
            { 
            
                versus.Status = "Closed";
            
                versus.InitiatorIterations = num;
                
                //Define winner
                if (botNum > versus.InitiatorIterations)
                    versus.WinnerName = "Bot";
                else if (versus.InitiatorIterations > botNum)
                    versus.WinnerName = versus.InitiatorName;
                else
                    versus.WinnerName = "Drawn Game";
                
                // Fix results
                await FixUserResults(
                    versus.InitiatorId,
                    versus.WinnerName == versus.InitiatorName,
                    versus.Exercise,
                    num);

                _context.Entry(versus).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                try
                {
                    string winner;
                    winner = versus.WinnerName == "DrawnGame" ? "DrawnGame" : versus.WinnerName;
                    
                    //Send messages
                    var socketInitId = await UserToSocket(versus.InitiatorId);
                    if(socketInitId != null)
                        await _messagesHandler.SendMessageAsync(socketInitId, 
                            JsonSerializer.Serialize(
                                new Dictionary<string,string>()
                                {
                                    {"Type", "Result"},
                                    {"Winner", winner}
                                }));
                    return Ok();
                    
                }
                catch(Exception ex)
                {
                    return StatusCode(500, ex);
                }
            }
            else
                return StatusCode(409, "Неподходящий статус поединка");

        }

        private async Task FixVersusResults(Data.Entities.Versus versus)
        {
            try
            {
                await FixUserResults(
                    versus.InitiatorId,
                    versus.WinnerName == versus.InitiatorName || versus.WinnerName == "DrawnGame",
                    versus.Exercise,
                    versus.InitiatorIterations
                );
                await FixUserResults(
                    versus.OpponentId,
                    versus.WinnerName == versus.OpponentName  || versus.WinnerName == "DrawnGame",
                    versus.Exercise,
                    versus.OpponentIterations
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fixing results after versus: " + ex.Message);
            }
            
        }
        private async Task FixUserResults(Guid userId, bool isWin, string ex, int num)
        {
            if (ex == "pushups")
            {
                var user = await _userManager.Users
                    .Include(u => u.Exercises)
                    .ThenInclude(e => e.PushUps)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (isWin)
                    user.Exercises.PushUps.Wins++;
                else 
                    user.Exercises.PushUps.Losses++;
                if (num > user.Exercises.PushUps.HighScore)
                    user.Exercises.PushUps.HighScore = num;
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
            }
            else if (ex == "pullups")
            {
                var user = await _userManager.Users
                    .Include(u => u.Exercises)
                    .ThenInclude(e => e.PullUps)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (isWin)
                    user.Exercises.PullUps.Wins++;
                else 
                    user.Exercises.PullUps.Losses++;
                if (num > user.Exercises.PullUps.HighScore)
                    user.Exercises.PullUps.HighScore = num;
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
            }
            else if (ex == "abs")
            {
                var user = await _userManager.Users
                    .Include(u => u.Exercises)
                    .ThenInclude(e => e.Abs)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (isWin)
                    user.Exercises.Abs.Wins++;
                else 
                    user.Exercises.Abs.Losses++;
                if (num > user.Exercises.Abs.HighScore)
                    user.Exercises.Abs.HighScore = num;
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                var user = await _userManager.Users
                    .Include(u => u.Exercises)
                    .ThenInclude(e => e.Squats)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (isWin)
                    user.Exercises.Squats.Wins++;
                else 
                    user.Exercises.Squats.Losses++;
                if (num > user.Exercises.Squats.HighScore)
                    user.Exercises.Squats.HighScore = num;
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
            }
        }
        private async Task<string> UserToSocket(Guid userId)
        {
            var socket = await _context.UserSockets
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (socket == null)
                return null;
            
            return socket.SocketId;
        }
        
    }
    public class IterationResponse
    {
        public string Type { get; set; }
        public int Num { get; set; }
    }
}