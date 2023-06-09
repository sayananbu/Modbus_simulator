﻿using Modbus.Data;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Modbus_simulator
{
    public class Wrapper:ObservableObject
    {
        public ConnectionState State { get; private set; }
        public DeviceModel Device1 { get; private set; }
        public DeviceModel Device2 { get; private set; }
        public DeviceModel Device3 { get; private set; }
        
        Thread flow;
        ModbusSlave slave = null;
        private bool isDisposed = true;
        public Wrapper()
        {
            var Top = (float)Math.Log10(2 * Math.Pow(10, 7));
            var Low = (float)Math.Log10(1 * Math.Pow(10,-1));
            State = new ConnectionState();
            Device1 = new DeviceModel(0, Top, Low);
            Device2 = new DeviceModel(1, Top, Low);
            Device3 = new DeviceModel(2, (float)Math.Log10(2 * Math.Pow(10, 4)), (float)Math.Log10(1 * Math.Pow(10, -2)));
        }
        public void DisposeConnection()
        {
            slave?.Dispose();
            isDisposed= true;
        }
        public void CreateConnection(string port, string baud)
        {
            if (port.Length != 0 && isDisposed)
            {
                flow = new Thread(() => SlaveThread(port, baud));
                flow.Start();
                isDisposed = false;
            }
            Thread.Sleep(50);
        }
        private void SlaveThread(string port, string baud)
        {
            try
            {
                using (SerialPort slavePort = new SerialPort(port, int.Parse(baud), Parity.None, 8, StopBits.One))
                {
                    slavePort.Open();
                    State.SetSuccess();
                    byte unitID = 1;
                    slave = ModbusSerialSlave.CreateRtu(unitID, slavePort);
                    DeviceModel.InitSlave(slave);
                    Device1.UpdateValues();
                    Device2.UpdateValues();
                    Device3.UpdateValues();
                    slave.Listen();
                    State.SetError();
                }
            }
            catch
            {
                State.SetError();
            }
        }
        
    }
}
