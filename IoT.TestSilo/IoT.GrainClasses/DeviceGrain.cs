using System;
using System.Threading.Tasks;
using IoT.GrainInterfaces;
using Orleans;

namespace IoT.GrainClasses
{
    public class DeviceGrain: Grain, IDeviceGrain
    {
        private double lastValue;

        public override Task OnActivateAsync()
        {
            Console.WriteLine($"Activated {this.GetPrimaryKeyLong()}");
            return base.OnActivateAsync();
        }

        public Task SetTemperature(double value)
        {
            if (lastValue < 100 && value >= 100)
            {
                Console.WriteLine($"High temperature recorded {value}");
            }

            lastValue = value;
            return Task.CompletedTask;
        }
    }
}