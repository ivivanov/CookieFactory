using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

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
            var synchronizers = new MachineModulesSynchronizers();
            var machine = new MachineModulesController(messageManager, synchronizers);

            machine.Start();

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
            //keep console open 
            while (true) ;
        }
    }
}
