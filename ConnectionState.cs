using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Modbus_simulator
{
    public class ConnectionState: ObservableObject
    {
        private SolidColorBrush _connectionColorState = new SolidColorBrush(Color.FromRgb(245, 139, 139));
        private SolidColorBrush _errorState = new SolidColorBrush(Color.FromRgb(245, 139, 139));
        private SolidColorBrush _successState = new SolidColorBrush(Color.FromRgb(194, 243, 190));
        public SolidColorBrush ConnectionColorState { get { return _connectionColorState; }
            set { 
                _connectionColorState = value; 
                OnPropertyChanged("ConnectionColorState");
            }
        }
        public void SetSuccess()
        {
            ConnectionColorState = _successState;
        }
        public void SetError()
        {
            ConnectionColorState = _errorState;
        }
    }
}
