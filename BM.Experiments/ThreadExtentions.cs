using System;
using System.Threading;

namespace BM.Experiments
{
    public static class ThreadExtentions
    {
        public static void PrintMessage(this Thread t, string m)
        {
            Console.WriteLine($"{t.Name} : {m}");
        }

        public static void PrintMessage(this Thread t, int i)
        {
            t.PrintMessage(i.ToString());
        }
    }
}
