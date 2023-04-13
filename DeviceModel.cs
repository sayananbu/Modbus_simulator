using Modbus.Data;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

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
        public DeviceModel(int deviceId, float maxRange, float minRange, DataStore dataStorage)
        {
            _deviceId = deviceId;
            MaxRange = maxRange;
            MinRange = minRange;
            _ready = true;
            _data = dataStorage;
        }
        private DataStore _data { get; set; }
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
        public float FullValue { get { return _value; } }
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
        public float FullSetpoint { get { return _setpoint; } }
        public float Setpoint
        {
            get { return (float)Math.Log10(_setpoint); }
            private set {
                if (!_setpointHold)
                {
                    _setpoint = value;
                    OnPropertyChanged("Setpoint");
                    OnPropertyChanged("FullSetpoint");
                }
            }
        }
        private void CalcValue(float mantisse, int pow)
        {
            float val = mantisse * (float)(Math.Pow(10, pow)) > float.MaxValue ? float.MaxValue : mantisse * (float)(Math.Pow(10, pow));
            Value = val;
            ApplyValueToRegister(val);
            Setpoint = val + val * 0.2f;
            ColorChange = _setpoint >= val ? _greenColor : _yellowColor;
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
        private bool IsValidIndex(int index, int size)
        {
            return index+1 <= size - 1 && index >=0 ? true: false;
        }
        private void ApplyValueToRegister(float value)
        {
            int register = 102 + _deviceId * 10;
            if(IsValidIndex(register, _data?.HoldingRegisters.Count))
            {
                var bytesFloat = BitConverter.GetBytes(value);
                var bytes = new Int16[] {
                    BitConverter.ToInt16(bytesFloat, 0),
                    BitConverter.ToInt16(bytesFloat, 2),
                };
                for (int i = 0; i < bytes.Length; i++)
                {
                    _data.HoldingRegisters[register + i] = (ushort)bytes[i];
                }
            }
        }

    }
}
