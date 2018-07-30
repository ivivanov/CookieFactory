using System;

namespace BM.Experiments
{
    public class Program
    {
        static void Main()
        {
            var machine = new MachineController().Init();

            machine.Start();

            while (true)
            {
                var command = Console.ReadLine();
                Console.WriteLine(command);
                machine.Pause();
            }
        }
    }
}
