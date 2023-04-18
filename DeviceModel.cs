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
        public DeviceModel(int deviceId, float maxRange, float minRange)
        {
            _deviceId = deviceId;
            MaxRange = maxRange;
            MinRange = minRange;
            _ready = true;
            CalcValue(_mantisse, _power);
        }
        public static void InitSlave(ModbusSlave slave)
        {
            _slave= slave;
        }
        private static ModbusSlave _slave = null;
        private static byte[] _wrnBits = new byte[3];
        private static byte[] _rdyBits = new byte[3];
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
            ApplySetpointToRegister(_setpoint);
            ApplyWarningState();
            ApplyReadyState();
            ColorChange = _setpoint > val ? _greenColor : _yellowColor;
        }
        public void UpdateValues()
        {
            CalcValue(_mantisse, _power);
        }

        private bool _ready;
        public bool Ready
        {
            get { return _ready; }
            set { _ready = value;
                ApplyReadyState();
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
        private void ApplyWarningState()
        {
            if (_value >= _setpoint) _wrnBits[_deviceId] = 1;
            else _wrnBits[_deviceId] = 0;
            if (_slave != null)
            {
                _slave.DataStore.HoldingRegisters[0xA] = (ushort)FromBitArrayToByte(_wrnBits);
            }
        }
        private void ApplyReadyState()
        {
            if (!_ready) _rdyBits[_deviceId] = 1;
            else _rdyBits[_deviceId] = 0;
            if (_slave != null)
            {
                var bytes = BitConverter.GetBytes((ushort)FromBitArrayToByte(_rdyBits)).Reverse().ToArray();
                _slave.DataStore.HoldingRegisters[1] = (ushort)BitConverter.ToInt16(bytes,0);
            }
        }
        private byte FromBitArrayToByte(byte[] array)
        {
            byte result = 0;
            for (int i = 0, c = 7; i < array.Length; i++,c--)
            {
                result += (byte)(array[i] * Math.Pow(2,7-c));
            }
            return result;
        }
        private float ToFourDigits(float value)
        {
            int lgth = Math.Truncate(Math.Abs(value)).ToString().Length;
            return lgth > 4 ? 0 : (float)Math.Round(value, 4 - lgth);
        }
        private void ApplyValueToRegister(float value)
        {
            int register = 0x103 + _deviceId * 0x010;
            var bytesFloat = BitConverter.GetBytes(value).Reverse().ToArray();
            var bytes = new Int16[] {
                    BitConverter.ToInt16(bytesFloat, 0),
                    BitConverter.ToInt16(bytesFloat, 2),
                };
            if (_slave != null)
            {
                _slave.DataStore.HoldingRegisters[register] = (ushort)bytes[0];
                _slave.DataStore.HoldingRegisters[register + 1] = (ushort)bytes[1];
            }
        }
        private void ApplySetpointToRegister(float value)
        {
            int register = 0x205 + _deviceId * 0x010;
            var bytesFloat = BitConverter.GetBytes(value).Reverse().ToArray();
            var bytes = new Int16[] {
                    BitConverter.ToInt16(bytesFloat, 0),
                    BitConverter.ToInt16(bytesFloat, 2),
                };
            if (_slave != null)
            {
                _slave.DataStore.HoldingRegisters[register] = (ushort)bytes[0];
                _slave.DataStore.HoldingRegisters[register + 1] = (ushort)bytes[1];
            }
        }

    }
}
