using System;
using System.Collections.Generic;
using System.Text;

namespace BM.MachineController.Modules
{
    public interface IModule
    {
        void Start();

        void Pause();

        void Stop();
    }
}
