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

        public abstract void Pause();

        public abstract void Start();

        public abstract void Stop();

        public virtual void DispatchMessage(string message)
        {
            Thread.CurrentThread.PrintMessage(message);
            messageProvider.Send($"{Name}:{message}");
        }
    }
}
