using System;
using System.Threading;

namespace BM.Experiments
{

    public class MachineController
    {
        private ManualResetEventSlim ovenIsReadyEvent;
        private AutoResetEvent motorPulseEvent;

        private CancellationTokenSource cancelSource;

        private Thread motorThread;
        private Thread biscuitMakerThread;
        private Thread ovenThread;

        private OvenModule ovenModule;
        private BiscuitMakerModule biscuitMakerModule;
        private MotorModule motorModule;

        public MachineController()
        {
            ovenIsReadyEvent = new ManualResetEventSlim(false);
            motorPulseEvent = new AutoResetEvent(false);
            cancelSource = new CancellationTokenSource();

            this.ovenModule = new OvenModule(ovenIsReadyEvent);
            this.biscuitMakerModule = new BiscuitMakerModule(motorPulseEvent);
            this.motorModule = new MotorModule(ovenIsReadyEvent, motorPulseEvent);
        }

        public MachineController Init()
        {
            Thread.CurrentThread.Name = nameof(MachineController);

            motorThread = new Thread(() =>
            {
                try { motorModule.Start(); }
                catch (OperationCanceledException)
                {
                }
            });
            motorThread.Name = nameof(motorThread);

            biscuitMakerThread = new Thread(() =>
            {
                try { biscuitMakerModule.Start(); }
                catch (OperationCanceledException)
                {
                }
            });
            biscuitMakerThread.Name = nameof(biscuitMakerThread);

            ovenThread = new Thread(() =>
            {
                try { ovenModule.Start(); }
                catch (OperationCanceledException)
                {
                }
            });
            ovenThread.Name = nameof(ovenThread);

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
        }

        public void Start()
        {
            Thread.CurrentThread.PrintMessage("Start");
            ovenThread.Start();
            biscuitMakerThread.Start();
            motorThread.Start();
        }
    }
}
