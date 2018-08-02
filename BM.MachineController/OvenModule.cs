using System;
using System.Threading;

namespace BM.MachineController
{
    public class OvenModule
    {
        private const int MaxTemperature = 240;
        private const int MinTemperature = 220;

        private CancellationTokenSource cancellation;
        private ManualResetEventSlim reachMaxTempEvent;
        private ManualResetEventSlim reachMinTempEvent;

        private Thread ovenThread;
        private Thread heatingThread;
        private Thread thermometerThread;

        private ManualResetEventSlim ovenIsReadyEvent;

        private int temperature;
        private bool isOvenReady;

        public OvenModule(ManualResetEventSlim ovenIsReadyEvent)
        {
            this.ovenIsReadyEvent = ovenIsReadyEvent;

            this.cancellation = new CancellationTokenSource();
            this.reachMaxTempEvent = new ManualResetEventSlim(false);
            this.reachMinTempEvent = new ManualResetEventSlim(true);

            ovenThread = new Thread(() =>
            {
                try { StartModules(); }
                catch (OperationCanceledException)
                {
                }
            });
            ovenThread.Name = nameof(ovenThread);

            heatingThread = new Thread(() =>
            {
                try { HeatOven(cancellation.Token); }
                catch (OperationCanceledException)
                {
                }
            });
            heatingThread.Name = $"{nameof(OvenModule)} / {nameof(heatingThread)}";

            thermometerThread = new Thread(() =>
            {
                try { MeasureTemperature(cancellation.Token); }
                catch (OperationCanceledException)
                {
                }
            });
            thermometerThread.Name = $"{nameof(OvenModule)} / {nameof(thermometerThread)}";
        }

        public void Start()
        {
            ovenThread.Start();
        }

        public void StartModules()
        {
            Thread.CurrentThread.PrintMessage("Start");
            heatingThread.Start();
            thermometerThread.Start();

            while (true)
            {
                if (MaxTemperature > temperature
                    && temperature > MinTemperature
                    && !isOvenReady)
                {
                    isOvenReady = true;
                    ovenIsReadyEvent.Set();
                }

                if ((temperature > MaxTemperature || temperature < MinTemperature)
                    && isOvenReady)
                {
                    isOvenReady = false;
                    ovenIsReadyEvent.Reset();
                }
            }
        }

        private void HeatOven(CancellationToken cancellationToken)
        {
            Thread.CurrentThread.PrintMessage("Start");

            while (true)
            {
                Thread.CurrentThread.PrintMessage("Heating...");
                reachMaxTempEvent.Reset();
                reachMaxTempEvent.Wait();
                Thread.CurrentThread.PrintMessage("Cooling...");
                reachMinTempEvent.Reset();
                reachMinTempEvent.Wait();
            }
        }

        private void MeasureTemperature(CancellationToken cancellationToken)
        {
            Thread.CurrentThread.PrintMessage("Start");
            bool isHeating = true;
            Random random = new Random();

            while (true)
            {
                Thread.CurrentThread.PrintMessage(temperature);
                if (temperature >= MaxTemperature)
                {
                    reachMinTempEvent.Reset();
                    reachMaxTempEvent.Set();
                    isHeating = false;
                }

                if (temperature <= MinTemperature)
                {
                    reachMaxTempEvent.Reset();
                    reachMinTempEvent.Set();
                    isHeating = true;
                }

                //fake some temperature measurements
                if (isHeating)
                {
                    //temperature++;
                    temperature += random.Next(1, 10);
                }
                else
                {
                    //temperature--;
                    temperature -= random.Next(1, 10);
                }

                Thread.Sleep(300);
            }
        }
    }
}
