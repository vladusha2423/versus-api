using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Core.EF;
using Versus.Data.Entities;

namespace Versus.WebSockets
{
    public class MessagesHandler : WebSocketHandler
    {
        // private readonly IJwtGenerator _jwt;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessagesHandler(ConnectionManager webSocketConnectionManager ,
            IServiceScopeFactory serviceScopeFactory
            // IJwtGenerator jwt
            ) 
            : base(webSocketConnectionManager)
        {
            // _jwt = jwt;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task OnConnectedWithUserId(WebSocket socket, Guid userId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<VersusContext>();
                var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                
                var user = await userManager.FindByIdAsync(userId.ToString());
                user.Online = true;
                if (await dbContext.UserSockets.AnyAsync(us => us.UserId == user.Id))
                    dbContext.Remove(
                        await dbContext.UserSockets
                            .FirstOrDefaultAsync(us => us.UserId == user.Id));
                
                await userManager.UpdateAsync(user);
                await dbContext.SaveChangesAsync();
                
                OnConnected(socket);

                var socketId = WebSocketConnectionManager.GetId(socket);

                await dbContext.UserSockets.AddAsync(new UserSocket
                {
                    UserId = userId,
                    SocketId = socketId
                });
                await dbContext.SaveChangesAsync();
                
                await SendMessageAsync(socketId, "{\"Type\": \"Connected\"}");
            }
            
        }
        
        public override async Task<bool> OnDisconnected(WebSocket socket)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<VersusContext>();
                var userSocket = await dbContext.UserSockets
                    .FirstOrDefaultAsync(us => us.SocketId == WebSocketConnectionManager.GetId(socket));

                if (userSocket != null)
                {
                    dbContext.UserSockets.Remove(userSocket);
                    
                    var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                    
                    var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userSocket.UserId);

                    if (user != null)
                    {
                        user.Online = false;
                        await userManager.UpdateAsync(user);


                        if (await dbContext.Versus.AnyAsync(v =>
                            v.LastInvitedId == user.Id && v.Status == "Searching" ||
                            (v.OpponentId == user.Id || v.InitiatorId == user.Id) && v.Status != "Closed"
                        ))
                        {
                            if (await dbContext.Versus.AnyAsync(v =>
                                v.LastInvitedId == user.Id && v.Status == "Searching"))
                            {
                                var versus = await dbContext.Versus
                                    .FirstOrDefaultAsync(v =>
                                        v.LastInvitedId == user.Id && v.Status == "Searching");
                                await FindJob(dbContext, userManager, versus.Exercise, versus.Id, 
                                    versus.InitiatorId == user.Id ? versus.OpponentName : versus.InitiatorName);
                            }
                            else if (await dbContext.Versus.AnyAsync(v =>
                                (v.InitiatorId == user.Id || v.OpponentId == user.Id) &&
                                v.Status == "Completion"))
                            {
                                var versus = await dbContext.Versus
                                    .FirstOrDefaultAsync(v =>
                                        (v.InitiatorId == user.Id || v.OpponentId == user.Id) &&
                                        v.Status == "Completion");
                                await CompleteVersus(dbContext, versus);
                            }
                            else
                            {
                                var versus = await dbContext.Versus
                                    .FirstOrDefaultAsync(v =>
                                        (v.InitiatorId == user.Id || v.OpponentId == user.Id) &&
                                        v.Status != "Closed");
                                versus.Status = "Canceled";
                                var socketId = await UserToSocket(dbContext,
                                    user.Id == versus.InitiatorId ? versus.OpponentId : versus.InitiatorId);
                                await SendMessageAsync(socketId, 
                                    JsonSerializer.Serialize(new Dictionary<string, string>
                                            {
                                                {"Type", "Cancel"}
                                            }));
                            }
                        }
                        
                        
                    }
                    
                    await dbContext.SaveChangesAsync();
                }

                
            }

            return await base.OnDisconnected(socket);
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";

            await SendMessageToAllAsync(message);
        }
        
        private async Task<int> Find(VersusContext context, UserManager<User> userManager, string exercise, 
            Guid versusId, string userName)
        {
            var user = await userManager.FindByNameAsync(userName);

            var versus = await context.Versus
                .Include(v => v.RejectedUser)
                .FirstOrDefaultAsync(v =>
                    v.Id == versusId);
            
            var opponent = await userManager.Users
                .FirstOrDefaultAsync(u => u.UserName != user.UserName && u.Online
                && !versus.RejectedUser.Select(rj => rj.UserId).Contains(u.Id));

            if (opponent == null)
            {
                versus.Status = "Canceled";
                context.Entry(versus).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return 404;
            }

            try
            {
                context.VersusUsers.Add(new VersusUser
                {
                    UserId = opponent.Id,
                    VersusId = versus.Id,
                }); 
                versus.LastInvitedId = opponent.Id;
                context.Entry(versus).State = EntityState.Modified;
                await context.SaveChangesAsync();
                var socketId = await UserToSocket(context, opponent.Id);
                if (socketId != null)
                {
                    await SendMessageAsync(socketId, 
                        JsonSerializer.Serialize(
                            new Dictionary<string, string>()
                            {
                                {"Type", "Invite"},
                                {"Exercise", exercise},
                                {"UserName", userName}
                            }));
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("An errof while send invite message " + ex.Message);
                return 500;
            }

            return 200;
        }

        public async Task FindJob(VersusContext context, UserManager<User> userManager, string exercise, 
            Guid versusId, string userName)
        {
            bool flag = true;
            while (flag)
            {
                var result = await Find(context, userManager, exercise, versusId, userName);
                if(result == 404)
                {
                    var versus = await context.Versus.FindAsync(versusId);
                    versus.Status = "BotReady";
                    await context.SaveChangesAsync();
                    
                    var socketId = await UserToSocket(context, versus.InitiatorId);
                    if (socketId != null)
                    {
                        await SendMessageAsync(socketId, 
                            JsonSerializer.Serialize(
                                new Dictionary<string, string>
                                {
                                    {"Type", "NotFound"}
                                }));
                    }

                    flag = false;
                }
                else if (result == 200)
                {
                    flag = false;
                }
                
            }
        }
        private async Task<string> UserToSocket(VersusContext context, Guid userId)
        {
            var socket = await context.UserSockets
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (socket == null)
                return null;
            
            return socket.SocketId;
        }

        private async Task CompleteVersus(VersusContext context, Data.Entities.Versus versus)
        {
            versus.Status = "Closed";
            
            if (versus.OpponentIterations > versus.InitiatorIterations)
                versus.WinnerName = versus.OpponentName;
            else if (versus.InitiatorIterations > versus.OpponentIterations)
                versus.WinnerName = versus.InitiatorName;
            else
                versus.WinnerName = "Drawn Game";
            
            context.Entry(versus).State = EntityState.Modified;
            await context.SaveChangesAsync();
            
            try
            {
                string winner;
                winner = versus.WinnerName == "DrawnGame" ? "DrawnGame" : versus.WinnerName;
                
                var socketInitId = await UserToSocket(context, versus.InitiatorId);
                var socketOppId = await UserToSocket(context, versus.OpponentId);
                if(socketInitId != null)
                    await SendMessageAsync(socketInitId, 
                        JsonSerializer.Serialize(
                            new Dictionary<string,string>()
                            {
                                {"Type", "Result"},
                                {"Winner", winner}
                            }));
                if(socketOppId != null)
                    await SendMessageAsync(socketOppId, 
                        JsonSerializer.Serialize(
                            new Dictionary<string,string>()
                            {
                                {"Type", "Result"},
                                {"Winner", winner}
                            }));
                
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while complete versus by reason of unexpected disconnect of member: " 
                                  + ex.Message);
            }
        }
        
        
    }
}