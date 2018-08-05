using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class BiscuitMakerModule : BaseModule
    {
        private readonly CountdownEvent rotationsCountdown;
        private readonly AutoResetEvent newBiscuitOnTheLine;
        private readonly ManualResetEventSlim lineIsEmptyEvent;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Thread biscuitMakerThread;

        public override string Name => nameof(BiscuitMakerModule);

        public BiscuitMakerModule(MachineModulesSynchronizers synchronizers, BiscuitCounterState biscuitCounterState, IMessageIOProvider message) : base(message)
        {
            this.rotationsCountdown = synchronizers.rotationsCountdownEvent;
            this.newBiscuitOnTheLine = synchronizers.newBiscuitOnTheLineEvent;
            this.lineIsEmptyEvent = synchronizers.lineIsEmptyEvent;
            this.cancellationTokenSource = synchronizers.cancellationTokenSource;
            biscuitMakerThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            base.Start();
            biscuitMakerThread.Start();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                BiscuitMakerJob(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void BiscuitMakerJob(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    lineIsEmptyEvent.Wait();
                    DispatchTurnedOffMessage();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                rotationsCountdown.Wait();
                rotationsCountdown.Reset();
                DispatchMessage("Extruder puts 1 biscuit");
                rotationsCountdown.Wait();
                rotationsCountdown.Reset();
                DispatchMessage("Stamper stamps 1 biscuit");
                newBiscuitOnTheLine.Set();
            }
        }
    }
}
