using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BM.Websockets.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IServiceProvider ServiceProvider { get; set; }

        public Startup(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration;
            ServiceProvider = serviceProvider;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseWebSockets();
            app.UseWebsocketsMiddleware("", serviceProvider.GetService<WebsocketHandler>());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketManager();
        }
    }
}
