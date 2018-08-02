using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using BM.MachineController;

namespace BM.Websockets.Server
{
    class Program
    {

        static void Main()
        {
            Console.WriteLine("Starting web socket server");

            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://*:5006")
                .Build();

            IServiceProvider serviceProvider = webHostBuilder.Services;

            webHostBuilder.Start();

            Console.WriteLine($"Ready");

            var messageManager = new MessageIOProvider(serviceProvider);
            var machine = new MachineModule(messageManager);

            machine.Start();

            var websocketHandler = serviceProvider.GetService<WebsocketHandler>();

            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        while (messageManager.HaveMessages)
                        {
                            websocketHandler.SendMessageAsync(messageManager.GetMessage().Data);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }).Start();

            //keep console open 
            while (true) ;
        }
    }
}
