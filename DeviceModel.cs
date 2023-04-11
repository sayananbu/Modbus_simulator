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
    public class DeviceModel: ObservableObject
    {
        public DeviceModel(int deviceId, float maxRange, float minRange)
        {
            _deviceId = deviceId;
            MaxRange = maxRange;
            MinRange = minRange;
            _ready = true;
        }
        private int _deviceId;
        private SolidColorBrush _yellowColor = new SolidColorBrush(Color.FromRgb(255, 216, 0));
        private SolidColorBrush _greenColor = new SolidColorBrush(Color.FromRgb(6, 176, 37));
        private SolidColorBrush _colorChange = new SolidColorBrush(Color.FromRgb(6, 176, 37));
        public SolidColorBrush ColorChange
        {
            get { return _colorChange; }
            private set
            {
                _colorChange = value;
                OnPropertyChanged("ColorChange");
            }
        }
        public float MaxRange { get; private set; }
        public float MinRange { get; private set; }
        private float _mantisse;

        public float Mantisse
        {
            get { return _mantisse; }
            set { 
                _mantisse = ToFourDigits(value);
                CalcValue(value, _power);
                OnPropertyChanged("Value");
            }
        }
        private float _value;
        public float Value
        {
            get { return (float)Math.Log10(_value); }
            set { 
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private int _power;

        public int Power
        {
            get { return _power; }
            set { 
                _power = value;
                CalcValue(_mantisse, value);
                OnPropertyChanged("Power");
            }
        }
        private float _setpoint;
        public float StrSetpoint { get { return _setpoint; } }
        public float Setpoint
        {
            get { return (float)Math.Log10(_setpoint); }
            private set {
                if (!_setpointHold)
                {
                    _setpoint = value;
                    OnPropertyChanged("Setpoint");
                    OnPropertyChanged("StrSetpoint");
                }
            }
        }
        private void CalcValue(float mantisse, int pow)
        {
            float val = _mantisse * (float)(Math.Pow(10, _power)) > float.MaxValue ? float.MaxValue : _mantisse * (float)(Math.Pow(10, _power));
            Value = val;
            Setpoint = val + val * 0.2f;
            ColorChange = _setpoint >= _value ? _greenColor : _yellowColor;
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
                if(!value) CalcValue(_mantisse, _power);
                OnPropertyChanged("SetpointHold");
            }
        }
        
        private float ToFourDigits(float value)
        {
            int lgth = Math.Truncate(Math.Abs(value)).ToString().Length;
            return lgth > 4 ? 0 : (float)Math.Round(value, 4 - lgth);
        }

    }
}
