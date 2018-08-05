using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class ThermometerModule : BaseModule
    {
        private readonly ManualResetEventSlim reachMaxTempEvent;
        private readonly ManualResetEventSlim reachMinTempEvent;
        private readonly ManualResetEventSlim lineIsEmptyEvent;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly TemperatureState temperatureState;
        private readonly Thread thermometerThread;

        public override string Name => nameof(ThermometerModule);

        public const int MaxTemperature = MachineConfig.MaxTemperature;
        public const int MinTemperature = MachineConfig.MinTemperature;

        public ThermometerModule(
            TemperatureState temperatureState,
            MachineModulesSynchronizers synchronizers,
            IMessageIOProvider message) : base(message)
        {
            this.temperatureState = temperatureState;
            this.reachMaxTempEvent = synchronizers.reachMaxTempEvent;
            this.reachMinTempEvent = synchronizers.reachMinTempEvent;
            this.lineIsEmptyEvent = synchronizers.lineIsEmptyEvent;
            this.cancellationTokenSource = synchronizers.cancellationTokenSource;
            thermometerThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            base.Start();
            thermometerThread.Start();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                MeasureTemperatureJob(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void MeasureTemperatureJob(CancellationToken cancellationToken)
        {
            bool isHeating = true;
            Random random = new Random();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    lineIsEmptyEvent.Wait();
                    DispatchMessage(0);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                int temperature = temperatureState.GetTemperature();
                DispatchMessage(temperature);

                if (temperature >= MaxTemperature)
                {
                    reachMinTempEvent.Reset();
                    reachMaxTempEvent.Set();
                    isHeating = false;
                }

                if (temperature <= MinTemperature)
                {
                    reachMaxTempEvent.Reset();
                    reachMinTempEvent.Set();
                    isHeating = true;
                }

                //fake some temperature measurements
                if (isHeating)
                {
                    temperatureState.UpdateTemperature(random.Next(1, 10));
                }
                else
                {
                    temperatureState.UpdateTemperature((-1) * random.Next(1, 10));
                }

                Thread.Sleep(1000);
            }
        }
    }
}
