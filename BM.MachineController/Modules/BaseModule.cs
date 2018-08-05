using BM.Common;
using System.Threading;

namespace BM.MachineController.Modules
{
    public abstract class BaseModule : IModule
    {
        private readonly IMessageIOProvider messageProvider;

        public BaseModule(IMessageIOProvider messageProvider)
        {
            this.messageProvider = messageProvider;
        }

        public abstract string Name { get; }

        public virtual void Pause() { }

        public virtual void Start()
        {
            DispatchTurnedOnMessage();
        }

        public virtual void Stop() { }

        public virtual void DispatchMessage(int message)
        {
            DispatchMessage(message.ToString());
        }

        public virtual void DispatchMessage(string message)
        {
            Thread.CurrentThread.PrintMessage(message);
            messageProvider.Send($"{Name}:{message}");
        }

        public virtual void DispatchTurnedOnMessage()
        {
            DispatchMessage(MachineConfig.TurnedOnMessage);
        }

        public virtual void DispatchTurnedOffMessage()
        {
            DispatchMessage(MachineConfig.TurnedOffMessage);
        }
    }
}
