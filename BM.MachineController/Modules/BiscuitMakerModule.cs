using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class BiscuitMakerModule : IModule
    {
        private readonly CountdownEvent rotationsCountdown;
        private readonly Thread biscuitMakerThread;
        private readonly IMessageIOProvider message;

        public string Name => nameof(BiscuitMakerModule);

        public BiscuitMakerModule(MachineModulesSynchronizers synchronizers, IMessageIOProvider message)
        {
            this.rotationsCountdown = synchronizers.rotationsCountdown;
            this.message = message;

            biscuitMakerThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            biscuitMakerThread.Start();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                Thread.CurrentThread.PrintMessage("Start");
                message.Send($"{Name}: start");
                BiscuitMakerJob();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void BiscuitMakerJob()
        {
            while (true)
            {
                rotationsCountdown.Wait();
                rotationsCountdown.Reset();
                Thread.CurrentThread.PrintMessage("Extruder puts 1 biscuit");
                message.Send($"{Name}: Extruder puts 1 biscuit");
                rotationsCountdown.Wait();
                rotationsCountdown.Reset();
                Thread.CurrentThread.PrintMessage("Stamper stamps 1 biscuit");
                message.Send($"{Name}: Stamper stamps 1 biscuit");
            }
        }
    }
}
