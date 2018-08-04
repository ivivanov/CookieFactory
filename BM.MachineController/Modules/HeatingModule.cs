using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class HeatingModule : BaseModule
    {
        private readonly ManualResetEventSlim reachMaxTempEvent;
        private readonly ManualResetEventSlim reachMinTempEvent;
        private readonly TemperatureState temperatureState;
        private readonly Thread heatingThread;

        public override string Name => nameof(HeatingModule);

        public HeatingModule(TemperatureState temperatureState, MachineModulesSynchronizers synchronizers, IMessageIOProvider message) : base(message)
        {
            this.temperatureState = temperatureState;
            this.reachMaxTempEvent = synchronizers.reachMaxTempEvent;
            this.reachMinTempEvent = synchronizers.reachMinTempEvent;
            heatingThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            heatingThread.Start();
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
                DispatchMessage("On");
                reachMaxTempEvent.Reset();
                reachMaxTempEvent.Wait();
                DispatchMessage("Off");
                reachMinTempEvent.Reset();
                reachMinTempEvent.Wait();
            }
        }
    }
}
