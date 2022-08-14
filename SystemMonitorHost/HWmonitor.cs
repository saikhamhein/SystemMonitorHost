using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace SystemMonitor
{
    internal sealed class CpuTemperatureReader : IDisposable
    {
        private readonly Computer _computer;

        public CpuTemperatureReader()
        {
            _computer = new Computer { CPUEnabled = true, GPUEnabled = true,  RAMEnabled = true};
            _computer.Open();
        }

        public (IReadOnlyDictionary<string, byte>, IReadOnlyDictionary<string, byte>) GetTemperaturesInCelsius()
        {
            var coreAndTemperature = new Dictionary<string, byte>();
            var coreAndLoad = new Dictionary<string, byte>();

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update(); //use hardware.Name to get CPU model
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                        coreAndTemperature.Add(sensor.Name, (byte)sensor.Value.Value);
                }

                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
                        coreAndLoad.Add(sensor.Name, (byte)sensor.Value.Value);
                }
            }

            return (coreAndTemperature,coreAndLoad);
        }

        public void Dispose()
        {
            try
            {
                _computer.Close();
            }
            catch (Exception)
            {
                //ignore closing errors
            }
        }
    }
}
