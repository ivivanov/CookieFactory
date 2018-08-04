using System;
using System.Collections.Generic;
using System.Text;

namespace BM.MachineController.Modules
{
    public interface IModule
    {
        string Name { get; }

        void Start();

        void Pause();

        void Stop();
    }
}
