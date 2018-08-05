using System.Threading;

namespace BM.MachineController
{
    /// <summary>
    /// Use it always as singleton
    /// All threads should share same instances of the synchronizators 
    /// </summary>
    public class MachineModulesSynchronizers
    {
        private const int RotationsPerBiscuit = 2;//total 4 rotations (2 extruder + 2 stamper)

        public readonly ManualResetEventSlim ovenIsReadyEvent;
        public readonly CountdownEvent rotationsCountdownEvent;
        public readonly ManualResetEventSlim reachMaxTempEvent;
        public readonly ManualResetEventSlim reachMinTempEvent;
        public readonly AutoResetEvent newBiscuitOnTheLineEvent;
        public readonly AutoResetEvent noBiscuitsOnTheLineEvent;
        public readonly ManualResetEventSlim lineIsEmptyEvent;
        public readonly CancellationTokenSource cancellationTokenSource;

        public MachineModulesSynchronizers()
        {
            this.ovenIsReadyEvent = new ManualResetEventSlim(false);
            this.rotationsCountdownEvent = new CountdownEvent(RotationsPerBiscuit);
            this.reachMaxTempEvent = new ManualResetEventSlim(false);
            this.reachMinTempEvent = new ManualResetEventSlim(true);
            this.newBiscuitOnTheLineEvent = new AutoResetEvent(false);
            this.noBiscuitsOnTheLineEvent = new AutoResetEvent(false);
            this.lineIsEmptyEvent = new ManualResetEventSlim(false);
            this.cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
