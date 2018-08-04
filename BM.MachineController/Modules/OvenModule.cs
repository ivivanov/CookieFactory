using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class OvenModule : BaseModule
    {
        private readonly ManualResetEventSlim ovenIsReadyEvent;
        private readonly HeatingModule heatingModule;
        private readonly ThermometerModule thermometerModule;
        private readonly Thread ovenThread;
        private readonly TemperatureState temperatureState;

        public const int MaxTemperature = MachineConfig.MaxTemperature;
        public const int MinTemperature = MachineConfig.MinTemperature;
        private bool isOvenReady;

        public override string Name => nameof(OvenModule);

        public OvenModule(
            HeatingModule heatingModule,
            ThermometerModule thermometerModule,
            TemperatureState temperatureState,
            IMessageIOProvider message,
            MachineModulesSynchronizers synchronizers) : base(message)
        {
            this.temperatureState = temperatureState;
            this.heatingModule = heatingModule;
            this.thermometerModule = thermometerModule;
            this.ovenIsReadyEvent = synchronizers.ovenIsReadyEvent;
            ovenThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            ovenThread.Start();
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                DispatchMessage("Start");
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
