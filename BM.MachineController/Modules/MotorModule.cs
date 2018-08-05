using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class MotorModule : BaseModule
    {
        private readonly ManualResetEventSlim ovenIsReadyEvent;
        private readonly CountdownEvent rotationsCountdown;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly BiscuitCounterState biscuitCounterState;
        private readonly Thread motorThread;

        public override string Name => nameof(MotorModule);

        public MotorModule(MachineModulesSynchronizers synchronizers, BiscuitCounterState biscuitCounterState, IMessageIOProvider message) : base(message)
        {
            this.ovenIsReadyEvent = synchronizers.ovenIsReadyEvent;
            this.rotationsCountdown = synchronizers.rotationsCountdownEvent;
            this.biscuitCounterState = biscuitCounterState;
            this.cancellationTokenSource = synchronizers.cancellationTokenSource;
            this.motorThread = new Thread(ThreadStartDelagate) { Name = Name };
            this.motorThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Start()
        {
            base.Start();
            motorThread.Start();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                ovenIsReadyEvent.Wait();
                RotationJob(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void RotationJob(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    while (biscuitCounterState.Count() > 0 )
                    {
                        RotateAndSignal();
                    }

                    DispatchTurnedOffMessage();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                RotateAndSignal();
            }
        }

        private void RotateAndSignal()
        {
            Thread.Sleep(1000); //simulate 1 full rotation of the motor gear
            DispatchMessage("Pulse...");
            rotationsCountdown.Signal();
        }
    }
}

