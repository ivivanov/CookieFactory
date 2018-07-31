using System;
using System.Threading;

namespace BM.Experiments
{
    public class BiscuitMakerModule
    {
        private CountdownEvent rotationsCountdown;
        private Thread biscuitMakerThread;

        public BiscuitMakerModule(CountdownEvent rotationsCountdown)
        {
            this.rotationsCountdown = rotationsCountdown;

            biscuitMakerThread = new Thread(() =>
            {
                try { BiscuitMaker(); }
                catch (OperationCanceledException)
                {
                }
            });
            biscuitMakerThread.Name = nameof(biscuitMakerThread);
        }

        public void Start()
        {
            biscuitMakerThread.Start();
        }

        private void BiscuitMaker()
        {
            Thread.CurrentThread.PrintMessage("Start");

            while (true)
            {
                rotationsCountdown.Wait();
                rotationsCountdown.Reset();
                Thread.CurrentThread.PrintMessage("Extruder puts 1 biscuit");
                rotationsCountdown.Wait();
                rotationsCountdown.Reset();
                Thread.CurrentThread.PrintMessage("Stamper stamps 1 biscuit");
            }
        }
    }
}
