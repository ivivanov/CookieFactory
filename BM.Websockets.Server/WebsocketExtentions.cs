using BM.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BM.Websockets.Server
{
    public static class WebsocketExtentions
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<WebsocketHandler>();

            return services;
        }


        public static IApplicationBuilder UseWebsocketsMiddleware(this IApplicationBuilder app, PathString path, WebsocketHandler handler)
        {
            return app.Map(path, x => x.UseMiddleware<WebsocketMiddleware>(handler));
        }
    }
}
