using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO.Ports;


namespace SystemMonitor
{
    internal class HWmonitor
    {
        private BackgroundWorker HWmonitorWorker;
        private CpuTemperatureReader HWInfo;
        private void HWmonitorWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            SerialPort serialPort = new SerialPort(e.Argument.ToString(), 9600, Parity.None, 8);
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            HWInfo = new CpuTemperatureReader();
            while (true)
            {
                var result = HWInfo.GetTemperaturesInCelsius();

                byte[] buf = new byte[5] { result.Item1["CPU Package"], result.Item1["GPU Core"], // Temperatures
                                           result.Item2["CPU Total"], result.Item2["GPU Core"], result.Item2["Memory"] // Loads
                                        }; 

                serialPort.Write(buf, 0, buf.Length);

                Thread.Sleep(2000);

                if(HWmonitorWorker.CancellationPending)
                {
                    Debug.WriteLine("stopping monitor");
                    serialPort.Close();
                    break;
                }
            }
        }

        public void MonitorStart(string serport)
        {   HWmonitorWorker = new BackgroundWorker();
            HWmonitorWorker.WorkerSupportsCancellation = true;
            HWmonitorWorker.DoWork += new DoWorkEventHandler(HWmonitorWorker_DoWork);
            HWmonitorWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MonitorCompleted);
            HWmonitorWorker.RunWorkerAsync(serport);
            Debug.WriteLine("Monitor started");
        }

        public void MonitorStop()
        {
            this.HWmonitorWorker.CancelAsync();
            
        }

        private void MonitorCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.HWmonitorWorker.DoWork -= new System.ComponentModel.DoWorkEventHandler(this.HWmonitorWorker_DoWork);
            this.HWInfo.Dispose();
            Debug.WriteLine("Monitor stopped");
        }

    }
}
