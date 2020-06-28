using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Versus.Data.Entities;
using Versus.Messaging.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Core.EF;
using Versus.WebSockets;

namespace Versus.DisconnectCheck
{
    [DisallowConcurrentExecution]
    public class DisconnectJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MessagesHandler _messagesHandler;
        
        public DisconnectJob(
            MessagesHandler mh, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _messagesHandler = mh;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private async Task SendMessage(string socketId)
        {
            await _messagesHandler.SendMessageAsync(socketId, "{\"Type\":\"CheckDisconnect\"}");
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<VersusContext>();
                var sockets = await dbContext.UserSockets.ToListAsync();
                foreach (var socket in sockets)
                {
                    var timeDiff = (DateTime.Now - socket.LastTime).TotalSeconds;
                    Console.WriteLine("TimeDiff: " + timeDiff);
                    if (timeDiff > 8)
                        await _messagesHandler.OnDisconnected(socket.SocketId);
                    await SendMessage(socket.SocketId);
                }
            }
            
        }
    }
}