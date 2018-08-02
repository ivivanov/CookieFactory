using BM.Common;
using System;
using System.Threading;

namespace BM.MachineController
{
    public class MachineModule
    {
        private const int RotationsPerBiscuit = 2;//total 4 rotations (2 extruder + 2 stamper)
        private readonly ManualResetEventSlim ovenIsReadyEvent;
        private readonly CountdownEvent rotationsCountdown;
        private readonly CancellationTokenSource cancelSource;
        private readonly OvenModule ovenModule;
        private readonly BiscuitMakerModule biscuitMakerModule;
        private readonly MotorModule motorModule;
        private readonly IMessageIOProvider message;

        public MachineModule(IMessageIOProvider message)
        {
            Thread.CurrentThread.Name = nameof(MachineModule);
            this.message = message;
            ovenIsReadyEvent = new ManualResetEventSlim(false);
            rotationsCountdown = new CountdownEvent(RotationsPerBiscuit);
            cancelSource = new CancellationTokenSource();

            this.ovenModule = new OvenModule(ovenIsReadyEvent);
            this.biscuitMakerModule = new BiscuitMakerModule(rotationsCountdown);
            this.motorModule = new MotorModule(ovenIsReadyEvent, rotationsCountdown);
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
            message.Send(new Message(MessageType.Text, $"{nameof(MachineModule)} started"));

            ovenModule.Start();
            motorModule.Start();
            biscuitMakerModule.Start();
        }
    }
}
