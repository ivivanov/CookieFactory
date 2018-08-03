using BM.Common;
using BM.MachineController.Modules;
using System.Threading;

namespace BM.MachineController
{
    public class MachineModulesController
    {
        private readonly IMessageIOProvider message;

        //private readonly CancellationTokenSource cancelSource;

        //machine modules
        private readonly IModule ovenModule;
        private readonly IModule biscuitMakerModule;
        private readonly IModule motorModule;


        public MachineModulesController(IMessageIOProvider message, MachineModulesSynchronizers synchronizers)
        {
            this.message = message;
            //this.cancelSource = synchronizers.cancelSource;
            //TODO
            TemperatureState temperatureState = new TemperatureState();

            this.ovenModule = new OvenModule(
                new HeatingModule(temperatureState, synchronizers, message),
                new ThermometerModule(temperatureState, synchronizers, message),
                temperatureState,
                message,
                synchronizers);

            this.biscuitMakerModule = new BiscuitMakerModule(synchronizers, message);
            this.motorModule = new MotorModule(synchronizers, message);
        }

        public void Pause()
        {
            //can pause only if started
            //cancelSource.Cancel();
        }

        public void Stop()
        {
            //can stop only if started
            //ovenIsReadyEvent.WaitHandle.Close();

        }

        public void Start()
        {
            ovenModule.Start();
            motorModule.Start();
            biscuitMakerModule.Start();
        }
    }
}
