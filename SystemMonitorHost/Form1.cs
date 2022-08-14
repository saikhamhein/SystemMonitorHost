using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SystemMonitor;
using System.IO.Ports;

namespace SystemMonitorHost
{
    public partial class Form1 : Form
    {
        private static HWmonitor hWmonitor;
        public Form1()
        {
            InitializeComponent();
            hWmonitor = new HWmonitor();
            
        }

        private void btn_scan_Click(object sender, EventArgs e)
        {
            cbox_port_list.Items.Clear();
            foreach(string s in SerialPort.GetPortNames())
            {
                cbox_port_list.Items.Add(s);
            }
            cbox_port_list.SelectedIndex = 0;
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"{cbox_port_list.SelectedItem} selected");
            if (btn_connect.Text == "Connect")
            {
                /*Start the hwmonitor and change btn text*/
                btn_connect.Text = "Disconnect";
                btn_scan.Enabled = false;
                hWmonitor.MonitorStart(cbox_port_list.SelectedItem.ToString());
            }
            else
            {
                /*Stop the hwmonitor and change btn text*/
                btn_connect.Text = "Connect";
                hWmonitor.MonitorStop();
                btn_scan.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbox_port_list.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                cbox_port_list.Items.Add(s);
            }
            cbox_port_list.SelectedIndex = 0;
        }
    }
}
