using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BM.WebGui
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        //await FakeLog(webSocket);
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute(
                    name: "default",
                    defaults: new { controller = "Machine", action = "Index" });
            });


        }

        private async Task FakeLog(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            Random random = new Random();
            bool end = false;

            while (!webSocket.CloseStatus.HasValue || end)
            {
                int randomInt = random.Next(1, 100000);

                if (randomInt == 999)
                {
                    end = true;
                }

                byte[] intBytes = BitConverter.GetBytes(randomInt);
                Array.Reverse(intBytes);
                buffer = intBytes;

                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, intBytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);

            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "999", CancellationToken.None);
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
