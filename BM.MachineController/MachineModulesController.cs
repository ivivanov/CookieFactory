using BM.MachineController.Modules;

namespace BM.MachineController
{
    public class MachineModulesController
    {
        private readonly IModule ovenModule;
        private readonly IModule biscuitMakerModule;
        private readonly IModule motorModule;


        public MachineModulesController(OvenModule ovenModule, BiscuitMakerModule biscuitMakerModule, MotorModule motorModule)
        {
            this.ovenModule = ovenModule;
            this.biscuitMakerModule = biscuitMakerModule;
            this.motorModule = motorModule;
        }

        public void Pause()
        {
            //TODO
            //can pause only if started
            //cancelSource.Cancel();
        }

        public void Stop()
        {
            //TODO
            //can stop only if started
            //ovenIsReadyEvent.WaitHandle.Close();
        }

        public void Start()
        {
            //TODO 
            //cant start twice
            ovenModule.Start();
            motorModule.Start();
            biscuitMakerModule.Start();
        }
    }
}
