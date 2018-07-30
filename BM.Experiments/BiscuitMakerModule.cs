using System.Threading;

namespace BM.Experiments
{
    public class BiscuitMakerModule
    {
        private AutoResetEvent motorPulseEvent;

        public BiscuitMakerModule(AutoResetEvent motorPulseEvent)
        {
            this.motorPulseEvent = motorPulseEvent;
        }

        public void Start()
        {
            BiscuitMaker();
        }

        private void BiscuitMaker()
        {
            Thread.CurrentThread.PrintMessage("Start");

            while (true)
            {
                motorPulseEvent.WaitOne();
                Thread.CurrentThread.PrintMessage("Extruder puts 1 biscuit");
                Thread.CurrentThread.PrintMessage("Stamper stamps 1 biscuit");
                //if (cancelToken.IsCancellationRequested)
                //{
                //    Thread.CurrentThread.PrintMessage("Closing...");

                //    cancelToken.ThrowIfCancellationRequested();
                //}
            }
        }
    }
}
