using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class HeatingModule : IModule
    {
        private readonly ManualResetEventSlim reachMaxTempEvent;
        private readonly ManualResetEventSlim reachMinTempEvent;
        private readonly IMessageIOProvider message;
        private readonly TemperatureState temperatureState;
        private readonly Thread heatingThread;

        public string Name => nameof(HeatingModule);

        public HeatingModule(TemperatureState temperatureState, MachineModulesSynchronizers synchronizers, IMessageIOProvider message)
        {
            this.temperatureState = temperatureState;
            this.reachMaxTempEvent = synchronizers.reachMaxTempEvent;
            this.reachMinTempEvent = synchronizers.reachMinTempEvent;
            this.message = message;

            heatingThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public void Start()
        {
            heatingThread.Start();
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
                HeatOvenJob();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void HeatOvenJob()
        {
            while (true)
            {
                Thread.CurrentThread.PrintMessage("Heating...");
                message.Send($"{Name}: Heating...");
                reachMaxTempEvent.Reset();
                reachMaxTempEvent.Wait();
                Thread.CurrentThread.PrintMessage("Cooling...");
                message.Send($"{Name}: Cooling...");
                reachMinTempEvent.Reset();
                reachMinTempEvent.Wait();
            }
        }
    }
}
