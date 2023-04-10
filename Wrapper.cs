using Modbus.Data;
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
        ModbusSlave slave;
        public Wrapper()
        {
            State = new ConnectionState();
        }
        public void DisposeConnection()
        {
            slave?.Dispose();
        }
        public void CreateConnection(string port, string baud)
        {
            slave?.Dispose();
            flow = new Thread(()=>SlaveThread(port,baud));
            flow.Start();
        }
        public void SlaveThread(string port, string baud)
        {
            try
            {
                using (SerialPort slavePort = new SerialPort(port, int.Parse(baud) , Parity.None, 8, StopBits.One))
                {
                    slavePort.Open();
                    State.SetSuccess();
                    byte unitID = 1;
                    slave = ModbusSerialSlave.CreateRtu(unitID, slavePort);
                    slave.DataStore = DataStoreFactory.CreateDefaultDataStore();
                    slave.Listen();
                }
            }
            catch
            {
                State.SetError();
            }

        }
    }
}
