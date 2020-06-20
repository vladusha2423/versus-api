using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Versus.WebSockets
{
    public abstract class WebSocketHandler
    {
        protected ConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(ConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public void OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);
        }

        public virtual async Task<bool> OnDisconnected(WebSocket socket)
        {
            try
            {
                await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if(socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                  offset: 0, 
                                                                  count: message.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);          
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            try
            {
                await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach(var pair in WebSocketConnectionManager.GetAll())
            {
                if(pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        //TODO - decide if exposing the message string is better than exposing the result and buffer
        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}