using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Modbus_simulator
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
    class DeviceModel: ObservableObject
    {
        private static float ToFourDigits(float value)
        {
            int lgth = Math.Truncate(Math.Abs(value)).ToString().Length;
            return lgth > 4 ? 0 : (float)Math.Round(value, 4 - lgth);
        }
        private SolidColorBrush _colorChange;
        private SolidColorBrush _yellowColor = new SolidColorBrush(Color.FromRgb(255, 216, 0));
        private SolidColorBrush _greenColor = new SolidColorBrush(Color.FromRgb(6, 176, 37));
        public SolidColorBrush ColorChange
        {
            get { return _colorChange; }
            set
            {
                _colorChange = value;
                OnPropertyChanged("ColorChange");
            }
        }
        private float _value;

        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private bool _ready;
        public bool Ready
        {
            get { return _ready; }
            set { _ready = value;
                OnPropertyChanged("Ready");
                }
        }

        private bool _setpointHold;
        public bool SetpointHold
        {
            get { return _setpointHold; }
            set { _setpointHold = value;
                OnPropertyChanged("SetpointHold");
            }
        }
        private float UpdateSetpoint()
        {
            return 1;
        }



    }
}
