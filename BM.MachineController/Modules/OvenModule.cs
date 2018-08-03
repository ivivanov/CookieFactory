using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class OvenModule : IModule
    {
        private readonly IMessageIOProvider message;
        private readonly ManualResetEventSlim ovenIsReadyEvent;
        private readonly HeatingModule heatingModule;
        private readonly ThermometerModule thermometerModule;
        private readonly Thread ovenThread;
        private readonly TemperatureState temperatureState;
        public readonly int MaxTemperature;
        public readonly int MinTemperature;
        private bool isOvenReady;

        public string Name => nameof(OvenModule);

        public OvenModule(
            HeatingModule heatingModule,
            ThermometerModule thermometerModule,
            TemperatureState temperatureState,
            IMessageIOProvider message,
            MachineModulesSynchronizers synchronizers)
        {
            this.temperatureState = temperatureState;
            this.heatingModule = heatingModule;
            this.thermometerModule = thermometerModule;
            this.ovenIsReadyEvent = synchronizers.ovenIsReadyEvent;
            this.message = message;
            MaxTemperature = MachineConfig.MaxTemperature;
            MinTemperature = MachineConfig.MinTemperature;
            ovenThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public void Start()
        {
            ovenThread.Start();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                Thread.CurrentThread.PrintMessage("Start");
                message.Send($"{Name}: start");
                StartSubModules();
                OvenJob();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void StartSubModules()
        {
            thermometerModule.Start();
            heatingModule.Start();
        }

        private void OvenJob()
        {
            while (true)
            {
                int temperature = temperatureState.GetTemperature();

                if (MaxTemperature > temperature
                    && temperature > MinTemperature
                    && !isOvenReady)
                {
                    isOvenReady = true;
                    ovenIsReadyEvent.Set();
                }

                if ((temperature > MaxTemperature || temperature < MinTemperature)
                    && isOvenReady)
                {
                    isOvenReady = false;
                    ovenIsReadyEvent.Reset();
                }
            }
        }
    }
}
