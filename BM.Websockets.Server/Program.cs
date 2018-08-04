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

            IServiceProvider serviceProvider = webHostBuilder.Services;

            webHostBuilder.Start();

            Console.WriteLine($"Ready");

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
                            break;
                        case "stop":
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
