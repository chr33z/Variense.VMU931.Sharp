using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using VMU931_Sharp.Data;
using VMU931_Sharp.Parser;

namespace VMU931_Sharp
{
    internal class SerialConnection
    {
        internal delegate void SerialDataReceivedHandler(VMU931_Frame dataFrame);

        internal SerialDataReceivedHandler SerialDataReceived;

        internal bool Connected { get; private set; }

        private SerialPort _port;

        private Queue<byte> receivedData = new Queue<byte>();

        #region Connection

        internal bool Connect(string port = "")
        {
            var devicePort = string.IsNullOrEmpty(port) ? FindDevicePort() : port;

            if(string.IsNullOrEmpty(devicePort))
            {
                return false;
            }
            else
            {
                _port = new SerialPort(devicePort, 9600);
                _port.DataReceived += SerialPort_DataReceived;
                _port.Open();
                Connected = true;
                return true;
            }
        }

        private string FindDevicePort()
        {
            var availablePorts = SerialPort.GetPortNames().ToList();

            foreach(string availablePort in availablePorts)
            {
                try
                {
                    using (SerialPort port = new SerialPort(availablePort, 9600))
                    {
                        port.ReadTimeout = 20000;
                        port.Open();

                        var data = new byte[128];
                        port.Read(data, 0, data.Length);

                        if(IsVMU931Data(data))
                        {
                            return availablePort;
                        }

                        port.Close();

                        return availablePort;
                    }
                }
                catch (System.Exception)
                {
                    continue;
                }
            }

            return null;
        }

        private bool IsVMU931Data(byte[] data)
        {
            Queue<byte> queue = new Queue<byte>();
            data.ToList().ForEach(b => queue.Enqueue(b));

            while (queue.Count >= 32)
            {
                var nextByte = queue.Dequeue();
                var followingByte = queue.Peek();
                if (nextByte == DataFrameParser.MessageEnd && followingByte == DataFrameParser.MessageStart)
                {
                    // maybe we found a data frame
                    var firstByte = queue.Dequeue();
                    var size = queue.Dequeue();
                    var type = (char)queue.Dequeue();

                    if (CommandType.IsCommandType(type)) return true;
                }
            }

            return false;
        }

        internal void Disconnect()
        {
            Connected = false;
            _port?.Close();
        }

        #endregion

        internal void SetMessageType(MessageType messgeType)
        {
            switch (messgeType)
            {
                case MessageType.EulerAngles:
                    _port.Write("var" + CommandType.EulerAngles);
                    break;
                case MessageType.Quaternions:
                    _port.Write("var" + CommandType.Quaternions);
                    break;
                case MessageType.Accelerometer:
                    _port.Write("var" + CommandType.Accelerometer);
                    break;
                case MessageType.Magnetometers:
                    _port.Write("var" + CommandType.Magnetometers);
                    break;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[_port.BytesToRead];
            _port.Read(data, 0, data.Length);
            data.ToList().ForEach(b => receivedData.Enqueue(b));

            ProcessData();
        }

        private void ProcessData()
        {
            while (receivedData.Count >= 32)
            {
                // skip to byte that indicates the start of a data frame
                bool lookingForFrame = true;
                while (lookingForFrame && receivedData.Count >= 32)
                {
                    var nextByte = receivedData.Dequeue();
                    var followingByte = receivedData.Peek();
                    if (nextByte == DataFrameParser.MessageEnd && followingByte == DataFrameParser.MessageStart)
                    {
                        lookingForFrame = false;
                        break;
                    }
                }

                if (lookingForFrame) continue;

                byte messageStart = receivedData.Dequeue();
                byte messageSize = receivedData.Dequeue();
                char messageType = (char)receivedData.Dequeue();

                // message size minus four reserved bytes
                byte[] data = new byte[messageSize - 4];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = receivedData.Dequeue();
                }

                byte messageEnd = receivedData.Dequeue();

                SerialDataReceived?.Invoke(DataFrameParser.ParseDateFrame(data, messageType));
            }
        }
    }
}
