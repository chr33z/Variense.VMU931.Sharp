using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using Variense.VMU931.Sharp.Data;
using Variense.VMU931.Sharp.Parser;
using Variense.VMU931.Sharp.Status;

namespace Variense.VMU931.Sharp
{
    internal class SerialConnection
    {
        internal delegate void DataFrameReceivedHandler(VMU931_Frame dataFrame);
        internal DataFrameReceivedHandler DataFrameReceived;

        internal delegate void StatusFrameReceivedHandler(VMU931_Status statusFrame);
        internal StatusFrameReceivedHandler StatusFrameReceived;

        internal bool Connected { get; private set; }

        private SerialPort _port;

        private string _lastPortName;

        private Queue<byte> receivedData = new Queue<byte>();

        #region Connection

        internal bool Connect(string port = "")
        {
            var devicePort = string.IsNullOrEmpty(port) ? FindDevicePort() : port;

            if (string.IsNullOrEmpty(devicePort))
            {
                _port = null;
                return false;
            }
            else
            {
                _port = new SerialPort(devicePort, 9600);
                _port.DataReceived += SerialPort_DataReceived;
                _port.Open();
                _lastPortName = devicePort;
                Connected = true;
                return true;
            }
        }

        private string FindDevicePort()
        {
            var availablePorts = SerialPort.GetPortNames().ToList();

            // if there is a already a port available that we should try then we can inject it as the first candiate in the list. 
            // Otherwise it will search on
            if(!string.IsNullOrEmpty(_lastPortName))
            {
                availablePorts.Insert(0, _lastPortName);
            }

            foreach (string availablePort in availablePorts)
            {
                try
                {
                    using (SerialPort p = new SerialPort(availablePort, 9600))
                    {
                        p.ReadTimeout = 100;
                        p.Open();

                        var data = new byte[128];
                        p.Read(data, 0, data.Length);

                        if (IsVMU931Data(data))
                        {
                            p.Close();
                            return availablePort;
                        }

                        p.Close();
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

            if(_port != null)
            {
                _port.DataReceived -= SerialPort_DataReceived;
                _port.Close();
            }
        }

        #endregion

        internal void SendMessageType(MessageType messgeType)
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
                case MessageType.Heading:
                    _port.Write("var" + CommandType.Heading);
                    break;
                case MessageType.Status:
                    _port.Write("var" + CommandType.Status);
                    break;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] data = new byte[_port.BytesToRead];
                _port.Read(data, 0, data.Length);
                data.ToList().ForEach(b => receivedData.Enqueue(b));

                ProcessData();
            }
            catch(Exception)
            {
                // TODO error handling
            }
        }

        private void ProcessData()
        {
            while (receivedData.Count >= 256)
            {
                // skip to byte that indicates the start of a data frame
                bool lookingForFrame = true;
                while (lookingForFrame && receivedData.Count >= 128)
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

                if (messageSize < 4) continue;

                // message size minus four reserved bytes
                byte[] data = new byte[messageSize - 4];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = receivedData.Dequeue();
                }

                byte messageEnd = receivedData.Dequeue();

                if (messageType == CommandType.Status)
                {
                    StatusFrameReceived?.Invoke(DataFrameParser.ParseStatusFrame(data));
                }
                else
                {
                    DataFrameReceived?.Invoke(DataFrameParser.ParseDateFrame(data, messageType));
                }
            }
        }

        internal void ClearBuffers()
        {
            _port?.DiscardInBuffer();
            _port?.DiscardOutBuffer();
        }
    }
}
