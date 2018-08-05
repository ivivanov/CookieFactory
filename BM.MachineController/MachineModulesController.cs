using System.Threading;
using BM.MachineController.Modules;

namespace BM.MachineController
{
    public class MachineModulesController
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IModule ovenModule;
        private readonly IModule biscuitMakerModule;
        private readonly IModule motorModule;
        private readonly IModule biscuitCounterModule;
        private bool isMachineStarted;

        public MachineModulesController(MachineModulesSynchronizers synchronizers, OvenModule ovenModule, BiscuitMakerModule biscuitMakerModule, MotorModule motorModule, BiscuitCounterModule biscuitCounterModule)
        {
            this.cancellationTokenSource = synchronizers.cancellationTokenSource;
            this.ovenModule = ovenModule;
            this.biscuitMakerModule = biscuitMakerModule;
            this.motorModule = motorModule;
            this.biscuitCounterModule = biscuitCounterModule;
        }

        public void Pause()
        {
            //TODO
            //can pause only if started
        }

        public void Stop()
        {
            if (isMachineStarted)
            {
                ovenModule.Stop();
                motorModule.Stop();
                biscuitMakerModule.Stop();
                biscuitCounterModule.Stop();
                isMachineStarted = false;
                cancellationTokenSource.Cancel();
            }
        }

        public void Start()
        {
            if (!isMachineStarted)
            {
                isMachineStarted = true;
                ovenModule.Start();
                motorModule.Start();
                biscuitMakerModule.Start();
                biscuitCounterModule.Start();
            }
        }
    }
}
