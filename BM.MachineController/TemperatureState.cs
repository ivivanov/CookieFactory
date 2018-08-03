namespace BM.MachineController
{
    public class TemperatureState
    {
        private readonly object locker = new object();
        private int temperature = 200;

        public int GetTemperature()
        {
            lock (locker)
            {
                return temperature;
            }
        }

        public void UpdateTemperature(int temperatureChange)
        {
            lock (locker)
            {
                temperature += temperatureChange;
            }
        }
    }
}
