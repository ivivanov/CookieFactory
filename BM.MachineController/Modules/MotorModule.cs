using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class MotorModule : IModule
    {
        private readonly ManualResetEventSlim ovenIsReadyEvent;
        private readonly CountdownEvent rotationsCountdown;
        private readonly Thread motorThread;
        private readonly IMessageIOProvider message;

        public string Name => nameof(MotorModule);

        public MotorModule(MachineModulesSynchronizers synchronizers, IMessageIOProvider message)
        {
            this.ovenIsReadyEvent = synchronizers.ovenIsReadyEvent;
            this.rotationsCountdown = synchronizers.rotationsCountdown;
            this.message = message;

            motorThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            motorThread.Start();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                ovenIsReadyEvent.Wait();
                Thread.CurrentThread.PrintMessage("Start");
                message.Send($"{Name}: start");
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
                Thread.CurrentThread.PrintMessage("Pulse...");
                message.Send($"{Name}: Pulse...");
                rotationsCountdown.Signal();
            }
        }
    }
}

