using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BM.Websockets.Server
{
    public class WebsocketMiddleware
    {
        private readonly RequestDelegate next;
        private WebsocketHandler webSocketHandler { get; set; }

        public WebsocketMiddleware(RequestDelegate next, WebsocketHandler webSocketHandler)
        {
            this.next = next;
            this.webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await webSocketHandler.OnConnected(socket);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await webSocketHandler.ReceiveAsync(socket, result, buffer);
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocketHandler.OnDisconnected(socket);
                    return;
                }

            });

            await next.Invoke(context);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
