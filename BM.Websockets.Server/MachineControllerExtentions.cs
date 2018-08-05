using BM.Common;
using BM.MachineController;
using BM.MachineController.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace BM.Websockets.Server
{
    public static class MachineControllerExtentions
    {
        public static IServiceCollection AddMachineModules(this IServiceCollection services)
        {
            services.AddSingleton<MachineModulesSynchronizers>();
            services.AddSingleton<TemperatureState>();
            services.AddSingleton<BiscuitCounterState>();

            services.AddTransient<MachineModulesController>();
            services.AddTransient<OvenModule>();
            services.AddTransient<HeatingModule>();
            services.AddTransient<ThermometerModule>();
            services.AddTransient<BiscuitMakerModule>();
            services.AddTransient<MotorModule>();
            services.AddTransient<BiscuitCounterModule>();

            return services;
        }

        public static IServiceCollection AddMessageIOProvider(this IServiceCollection services)
        {
            services.AddSingleton<IMessageIOProvider, MessageIOProvider>();
            return services;
        }
    }
}
