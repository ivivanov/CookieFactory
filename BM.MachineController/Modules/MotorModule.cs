using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class MotorModule : BaseModule
    {
        private readonly ManualResetEventSlim ovenIsReadyEvent;
        private readonly CountdownEvent rotationsCountdown;
        private readonly Thread motorThread;

        public override string Name => nameof(MotorModule);

        public MotorModule(MachineModulesSynchronizers synchronizers, IMessageIOProvider message) : base(message)
        {
            this.ovenIsReadyEvent = synchronizers.ovenIsReadyEvent;
            this.rotationsCountdown = synchronizers.rotationsCountdown;
            motorThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            motorThread.Start();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                ovenIsReadyEvent.Wait();
                DispatchMessage("Start");
                RotationJob();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void RotationJob()
        {
            while (true)
            {
                Thread.Sleep(1000); //simulate 1 full rotation of the motor gear
                DispatchMessage("Pulse...");
                rotationsCountdown.Signal();
            }
        }
    }
}

