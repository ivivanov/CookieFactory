namespace BM.MachineController
{
    public class BiscuitCounterState
    {
        private readonly object locker = new object();

        private int biscuitsOnTheLine;

        public int Count()
        {
            lock (locker)
            {
                return biscuitsOnTheLine;
            }
        }

        public void Increment()
        {
            lock (locker)
            {
                biscuitsOnTheLine++;
            }
        }

        public void Decrement()
        {
            lock (locker)
            {
                biscuitsOnTheLine--;
            }
        }
    }
}
