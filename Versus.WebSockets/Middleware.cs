using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Versus.Auth.Interfaces;

namespace Versus.WebSockets
{
    public class WebSocketManagerMiddleware
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly RequestDelegate _next;
        private readonly MessagesHandler _webSocketHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public WebSocketManagerMiddleware(RequestDelegate next, MessagesHandler webSocketHandler,
            IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [Authorize]
        public async Task Invoke(HttpContext context)
        {
            if(!context.WebSockets.IsWebSocketRequest)
                return;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var username = context.Request.Path.ToString().Split("&")[0].Replace("/username=", "");
                var password = context.Request.Path.ToString().Split("&")[1].Replace("password=", "");
                var authService = scope.ServiceProvider.GetService<IAuthService>();
                var result = await authService.Login(username, password);
                if (result == null)
                {
                    context.Response.StatusCode = 401;
                    return;
                }
                
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                await _webSocketHandler.OnConnectedWithUserId(socket, result.UserId); 
            
                await Receive(socket, async(resultMsg, buffer) =>
                {
                    if(resultMsg.MessageType == WebSocketMessageType.Text)
                    {
                        await _webSocketHandler.ReceiveAsync(socket, resultMsg, buffer);
                    }

                    else if(resultMsg.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocketHandler.OnDisconnected(socket);
                    }

                });
            }


            // if (!context.User.Identity.IsAuthenticated)
            // {
            //     context.Response.StatusCode = 401;
            //     return;
            // }
            
            
            
            //TODO - investigate the Kestrel exception thrown when this is the last middleware
            //await _next.Invoke(context);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while(socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);                
            }
        }
    }
}