using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

using BM.MachineController;
using BM.Common;

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

            webHostBuilder.Start();

            Console.WriteLine($"Ready");

            IServiceProvider serviceProvider = webHostBuilder.Services;
            var messageManager = serviceProvider.GetService<IMessageIOProvider>() as MessageIOProvider;
            var machine = serviceProvider.GetService<MachineModulesController>();
            var websocketHandler = serviceProvider.GetService<WebsocketHandler>();

            new Thread(() =>
            {
                while (true)
                {
                    while (messageManager.HaveOutgoingMessages)
                    {
                        websocketHandler.SendMessageAsync(messageManager.GetOutgoingMessage());
                    }
                }
            }).Start();

            while (true)
            {
                while (messageManager.HaveIncomingMessages)
                {
                    string message = messageManager.GetIncomingMessage();
                    switch (message)
                    {
                        case "start":
                            machine.Start();
                            break;
                        case "pause":
                            machine.Pause();
                            break;
                        case "stop":
                            machine.Stop();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
