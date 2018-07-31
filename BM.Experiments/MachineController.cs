using System;
using System.Threading;

namespace BM.Experiments
{

    public class MachineController
    {
        private const int RotationsPerBiscuit = 2;//total 4 rotations (2 extruder + 2 stamper)
        private ManualResetEventSlim ovenIsReadyEvent;
        private CountdownEvent rotationsCountdown;

        private CancellationTokenSource cancelSource;

        private OvenModule ovenModule;
        private BiscuitMakerModule biscuitMakerModule;
        private MotorModule motorModule;

        public MachineController()
        {
            ovenIsReadyEvent = new ManualResetEventSlim(false);
            rotationsCountdown = new CountdownEvent(RotationsPerBiscuit);
            cancelSource = new CancellationTokenSource();

            this.ovenModule = new OvenModule(ovenIsReadyEvent);
            this.biscuitMakerModule = new BiscuitMakerModule(rotationsCountdown);
            this.motorModule = new MotorModule(ovenIsReadyEvent, rotationsCountdown);
        }

        public MachineController Init()
        {
            Thread.CurrentThread.Name = nameof(MachineController);

            return this;
        }

        public void Pause()
        {
            //can pause only if started
            cancelSource.Cancel();
        }

        public void Stop()
        {
            //can stop only if started
            ovenIsReadyEvent.WaitHandle.Close();

        }

        public void Start()
        {
            Thread.CurrentThread.PrintMessage("Start");

            ovenModule.Start();
            motorModule.Start();
            biscuitMakerModule.Start();
        }
    }
}
