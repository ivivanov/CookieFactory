using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class OvenModule : BaseModule
    {
        private readonly ManualResetEventSlim ovenIsReadyEvent;
        private readonly ManualResetEventSlim lineIsEmptyEvent;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly TemperatureState temperatureState;

        private readonly Thread ovenThread;
        private readonly HeatingModule heatingModule;
        private readonly ThermometerModule thermometerModule;

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
            this.lineIsEmptyEvent = synchronizers.lineIsEmptyEvent;
            this.cancellationTokenSource = synchronizers.cancellationTokenSource;
            ovenThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            base.Start();
            ovenThread.Start();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                StartSubModules();
                OvenJob(cancellationTokenSource.Token);
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

        private void OvenJob(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    lineIsEmptyEvent.Wait();
                    DispatchTurnedOffMessage();
                    cancellationToken.ThrowIfCancellationRequested();
                }

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
