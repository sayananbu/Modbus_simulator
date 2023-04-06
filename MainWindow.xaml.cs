using Modbus.Data;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Modbus_simulator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread flow;
        ModbusSlave slave;
        public MainWindow()
        {
            InitializeComponent();
            flow = new Thread(SlaveThread);
            flow.Start();
        }
        public void SlaveThread()
        {
            using (SerialPort slavePort = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One))
            {
                slavePort.Open();
                byte unitID = 1;
                slave = ModbusSerialSlave.CreateRtu(unitID, slavePort);
                slave.DataStore = DataStoreFactory.CreateDefaultDataStore();
                slave.Listen();
            }
        }
        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            slave.Dispose();
        }
    }
}
