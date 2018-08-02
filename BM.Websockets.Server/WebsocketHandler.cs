using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BM.Websockets.Server
{
    public class WebsocketHandler
    {
        private WebSocket socket;

        public WebsocketHandler()
        {
        }

        public async Task SendMessageAsync(string message)
        {
            await SendMessageAsync(this.socket, message);
        }

        public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {

        }

        public async Task OnConnected(WebSocket socket)
        {
            this.socket = socket;

            await SendMessageAsync("connected");
        }

        public async Task OnDisconnected(WebSocket socket)
        {
            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Connection closed",
                                    cancellationToken: CancellationToken.None);
        }

        private async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                  offset: 0,
                                                                  count: message.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);
        }
    }
}
