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
        string[] bauds =
        {
            "2400", "4800", "9600", "14400", "19200", "38400", "56000", "57600"
        };
        public MainWindow()
        {
            InitializeComponent();
            InitComPorts();
        }
        
        private void InitComPorts()
        {
            var ports = SerialPort.GetPortNames().Where(item => item.Contains("COM"));
            foreach (var port in ports)
            {
                portsList.Items.Add(port);
            }
            portsList.SelectedIndex = 0;
            foreach(var baud in bauds)
            {
                baudRates.Items.Add(baud);
            }
            baudRates.SelectedIndex = 0;
        }
        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            ((Wrapper)this.DataContext).CreateConnection(portsList.SelectedItem.ToString() , baudRates.SelectedItem.ToString() );
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((Wrapper)this.DataContext).DisposeConnection();
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
