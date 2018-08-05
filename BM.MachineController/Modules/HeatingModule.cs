using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class HeatingModule : BaseModule
    {
        private readonly ManualResetEventSlim reachMaxTempEvent;
        private readonly ManualResetEventSlim reachMinTempEvent;
        private readonly ManualResetEventSlim lineIsEmptyEvent;
        private readonly TemperatureState temperatureState;
        private readonly BiscuitCounterState biscuitCounterState;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Thread heatingThread;

        public override string Name => nameof(HeatingModule);

        public HeatingModule(TemperatureState temperatureState, BiscuitCounterState biscuitCounterState, MachineModulesSynchronizers synchronizers, IMessageIOProvider message) : base(message)
        {
            this.temperatureState = temperatureState;
            this.biscuitCounterState = biscuitCounterState;
            this.reachMaxTempEvent = synchronizers.reachMaxTempEvent;
            this.reachMinTempEvent = synchronizers.reachMinTempEvent;
            this.lineIsEmptyEvent = synchronizers.lineIsEmptyEvent;
            this.cancellationTokenSource = synchronizers.cancellationTokenSource;
            heatingThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            heatingThread.Start();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                HeatOvenJob(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void HeatOvenJob(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    lineIsEmptyEvent.Wait();
                    DispatchTurnedOffMessage();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                DispatchTurnedOnMessage();
                reachMaxTempEvent.Reset();
                reachMaxTempEvent.Wait();
                DispatchTurnedOffMessage();
                reachMinTempEvent.Reset();
                reachMinTempEvent.Wait();
            }
        }
    }
}
