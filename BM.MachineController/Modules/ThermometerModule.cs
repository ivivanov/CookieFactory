using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class ThermometerModule : IModule
    {
        private readonly ManualResetEventSlim reachMaxTempEvent;
        private readonly ManualResetEventSlim reachMinTempEvent;
        private readonly IMessageIOProvider message;
        private readonly TemperatureState temperatureState;
        private readonly Thread thermometerThread;

        public string Name => nameof(ThermometerModule);

        public readonly int MaxTemperature;
        public readonly int MinTemperature;

        public ThermometerModule(
            TemperatureState temperatureState,
            MachineModulesSynchronizers synchronizers,
            IMessageIOProvider message)
        {
            this.temperatureState = temperatureState;
            this.reachMaxTempEvent = synchronizers.reachMaxTempEvent;
            this.reachMinTempEvent = synchronizers.reachMinTempEvent;
            this.message = message;

            MaxTemperature = MachineConfig.MaxTemperature;
            MinTemperature = MachineConfig.MinTemperature;

            thermometerThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public void Start()
        {
            thermometerThread.Start();
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
                Thread.CurrentThread.PrintMessage(temperature);
                message.Send($"{Name}: {temperature}");

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

                Thread.Sleep(300);
            }
        }
    }
}
