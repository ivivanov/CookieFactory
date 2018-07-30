using System.Threading;

namespace BM.Experiments
{
    public class MotorModule
    {
        private AutoResetEvent motorPulseEvent;
        private ManualResetEventSlim ovenIsReadyEvent;

        public MotorModule(ManualResetEventSlim ovenIsReadyEvent, AutoResetEvent motorPulseEvent)
        {
            this.motorPulseEvent = motorPulseEvent;
            this.ovenIsReadyEvent = ovenIsReadyEvent;
        }

        public void Start()
        {
            StartMotor();
        }

        private void StartMotor()
        {
            ovenIsReadyEvent.Wait();
            Thread.CurrentThread.PrintMessage("Start");

            while (true)
            {
                motorPulseEvent.Set();
                Thread.CurrentThread.PrintMessage("Pulse...");

                Thread.Sleep(1000); //simulate 1 full rotation of the motor gear
            }
        }
    }
}
