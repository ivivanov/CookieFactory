using BM.Common;
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
        private readonly IMessageIOProvider message;

        public WebsocketHandler(IMessageIOProvider message)
        {
            this.message = message;
        }

        public async Task SendMessageAsync(string message)
        {
            await SendMessageAsync(this.socket, message);
        }

        public void Receive(string text)
        {
            message.Receive(text);
        }

        public void OnConnected(WebSocket socket)
        {
            this.socket = socket;
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

            await socket.SendAsync(
                new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
