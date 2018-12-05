using System;
using System.Threading;
using Variense.VMU931.Sharp;
using Variense.VMU931.Sharp.Data;
using Variense.VMU931.Sharp.Status;

namespace Variense.VMU931.Sharp_ConsoleTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            VMU931_Device _vmu931_device = new VMU931_Device();

            _vmu931_device.DataFrameReceived += VMU932_DataFrameReceived;
            _vmu931_device.DeviceStatusReceived += VMU932_DeviceStatusReceived;
            _vmu931_device.Connect(true);

            _vmu931_device.RequestStatusUpdate();
            _vmu931_device.EnableMessageType(MessageType.Accelerometer);
            _vmu931_device.EnableMessageType(MessageType.Magnetometers);
            _vmu931_device.EnableMessageType(MessageType.Heading);
            _vmu931_device.EnableMessageType(MessageType.Quaternions);

            int i = 0;
            while (i < 30)
            {
                Thread.Sleep(250);
                _vmu931_device.RequestStatusUpdate();
            }

            _vmu931_device.Disconnect();
        }

        private static void VMU932_DataFrameReceived(VMU931_Frame frame)
        {
            Console.WriteLine(frame.ToString());
        }

        private static void VMU932_DeviceStatusReceived(VMU931_Status status)
        {
            Console.WriteLine(status.ToString());
        }
    }
}
