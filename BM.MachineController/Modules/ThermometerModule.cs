using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class ThermometerModule : BaseModule
    {
        private readonly ManualResetEventSlim reachMaxTempEvent;
        private readonly ManualResetEventSlim reachMinTempEvent;
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
            thermometerThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            thermometerThread.Start();
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
                MeasureTemperatureJob();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void MeasureTemperatureJob()
        {
            bool isHeating = true;
            Random random = new Random();

            while (true)
            {
                int temperature = temperatureState.GetTemperature();
                DispatchMessage(temperature.ToString());

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
