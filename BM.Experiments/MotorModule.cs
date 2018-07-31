using System;
using System.Threading;

namespace BM.Experiments
{
    public class MotorModule
    {
        private ManualResetEventSlim ovenIsReadyEvent;
        private CountdownEvent rotationsCountdown;
        private Thread motorThread;

        public MotorModule(ManualResetEventSlim ovenIsReadyEvent, CountdownEvent rotationsCountdown)
        {
            this.ovenIsReadyEvent = ovenIsReadyEvent;
            this.rotationsCountdown = rotationsCountdown;

            motorThread = new Thread(() =>
            {
                try { StartMotor(); }
                catch (OperationCanceledException)
                {
                }
            });
            motorThread.Name = nameof(motorThread);

        }

        public void Start()
        {
            motorThread.Start();
        }

        public void Stop()
        {

        }

        private void StartMotor()
        {
            ovenIsReadyEvent.Wait();
            Thread.CurrentThread.PrintMessage("Start");

            while (true)
            {
                Thread.Sleep(1000); //simulate 1 full rotation of the motor gear
                Thread.CurrentThread.PrintMessage("Pulse...");
                rotationsCountdown.Signal();
            }
        }
    }
}

