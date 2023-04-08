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
            try
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
            catch { }
            
        }
        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            slave?.Dispose();
        }
        private void CheckFloatNumberInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (!textBox.Text.Contains(".")
               && textBox.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }
        private void CheckIntNumberInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        private void RedirectFocus(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
        }
        private void CheckEmptyInput(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            string cleanText = textbox.Text.Replace(" ", string.Empty);
            textbox.Text = cleanText.Length != 0 ? cleanText : "0";
        }
    }
}
