using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController.Modules
{
    public class BiscuitMakerModule : BaseModule
    {
        private readonly CountdownEvent rotationsCountdown;
        private readonly Thread biscuitMakerThread;

        public override string Name => nameof(BiscuitMakerModule);

        public BiscuitMakerModule(MachineModulesSynchronizers synchronizers, IMessageIOProvider message) : base(message)
        {
            this.rotationsCountdown = synchronizers.rotationsCountdown;
            biscuitMakerThread = new Thread(ThreadStartDelagate) { Name = Name };
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            biscuitMakerThread.Start();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        private void ThreadStartDelagate()
        {
            try
            {
                DispatchMessage("Start");
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
                DispatchMessage("Extruder puts 1 biscuit");
                rotationsCountdown.Wait();
                rotationsCountdown.Reset();
                DispatchMessage("Stamper stamps 1 biscuit");
            }
        }
    }
}
