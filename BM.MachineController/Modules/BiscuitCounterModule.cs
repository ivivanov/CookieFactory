using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class BiscuitCounterModule : BaseModule
    {
        private readonly AutoResetEvent newBiscuitOnTheLine;
        private readonly AutoResetEvent noBiscuitsOnTheLine;
        private readonly CountdownEvent rotationsCountdown;
        private readonly ManualResetEventSlim lineIsEmptyEvent;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Thread biscuitCounterThread;
        private readonly BiscuitCounterState biscuitCounterState;
        private int bakedBiscuits;
        private const int MaxBiscuitsOnTheLine = MachineConfig.MaxBiscuitsOnTheLine;

        public override string Name => nameof(BiscuitCounterModule);

        public BiscuitCounterModule(MachineModulesSynchronizers synchronizers, BiscuitCounterState biscuitCounterState, IMessageIOProvider message) : base(message)
        {
            this.newBiscuitOnTheLine = synchronizers.newBiscuitOnTheLineEvent;
            this.noBiscuitsOnTheLine = synchronizers.noBiscuitsOnTheLineEvent;
            this.rotationsCountdown = synchronizers.rotationsCountdownEvent;
            this.lineIsEmptyEvent = synchronizers.lineIsEmptyEvent;
            this.biscuitCounterState = biscuitCounterState;
            this.cancellationTokenSource = synchronizers.cancellationTokenSource;
            biscuitCounterThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            base.Start();
            biscuitCounterThread.Start();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                BiscuitCounterJob(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void BiscuitCounterJob(CancellationToken cancellationToken)
        {
            while (true)
            {
                newBiscuitOnTheLine.WaitOne();

                if (cancellationToken.IsCancellationRequested)
                {
                    while (biscuitCounterState.Count() > 0)
                    {
                        rotationsCountdown.Wait();
                        rotationsCountdown.Reset();
                        biscuitCounterState.Decrement();
                        bakedBiscuits++;
                        DispatchMessage($"Baking:{biscuitCounterState.Count()}");
                        DispatchMessage($"Baked:{bakedBiscuits}");
                    }

                    DispatchTurnedOffMessage();
                    lineIsEmptyEvent.Set();
                    cancellationToken.ThrowIfCancellationRequested();
                }


                if (biscuitCounterState.Count() == MaxBiscuitsOnTheLine)
                {
                    biscuitCounterState.Decrement();
                    bakedBiscuits++;
                    DispatchMessage($"Baked:{bakedBiscuits}");
                }

                biscuitCounterState.Increment();
                DispatchMessage($"Baking:{biscuitCounterState.Count()}");
            }
        }
    }
}
