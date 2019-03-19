using System.Diagnostics;
using System.Threading;
using Variense.VMU931.Sharp.Data;
using Variense.VMU931.Sharp.Status;

namespace Variense.VMU931.Sharp
{
    public class VMU931_Device
    {
        public delegate void DataFrameReceivedHandler(VMU931_Frame frame);
        public DataFrameReceivedHandler DataFrameReceived;

        public delegate void DeviceStatusReceivedHandler(VMU931_Status status);
        public DeviceStatusReceivedHandler DeviceStatusReceived;

        public int MaxRetries { get; set; } = 5;

        public VMU931_Status DeviceStatus { get; private set; }

        public bool Connected {
            get { return _serialConnection != null && _serialConnection.Connected; }
        }

        private SerialConnection _serialConnection;

        private Thread _serialThread;

        private bool _connectionRunning = false;

        private bool _firstStatusUpdateReceived = false;

        /// <summary>
        /// Start a connection to the VMU931 which will immediatelly send data.
        /// Retrieve data by connecting to the <see cref="DataFrameReceived"/> event.
        /// The connection runs in its own thread, so please call Disconnect when you are finished
        /// </summary>
        /// <param name="retryConnection"></param>
        /// <param name="port"></param>
        public void Connect(bool retryConnection = true, string port = "")
        {
            if (_serialThread != null && _serialThread.IsAlive)
            {
                return;
            }
            else
            {
                _serialThread = new Thread(() => RunSerialConnection(retryConnection, port));
                _serialThread.Start();
            }
        }

        /// <summary>
        /// Disconnect from this device and stop streaming data
        /// </summary>
        public void Disconnect()
        {
            _connectionRunning = false;
            _serialThread?.Join();
        }

        private void RunSerialConnection(bool retryConnect = true, string port = "")
        {
            _connectionRunning = true;

            _serialConnection = new SerialConnection();
            _serialConnection.DataFrameReceived += SerialConnection_DataFrameReceived;
            _serialConnection.StatusFrameReceived += SerialConnection_StatusFrameReceived;

            int retries = MaxRetries;
            while (retries-- > 0)
            {
                if (_serialConnection.Connect(port)) break;
                Thread.Sleep(100);
            }

            if(!_serialConnection.Connected)
            {
                _connectionRunning = false;
                Debug.WriteLine("[VMU931_Device] Connection error.");
                return;
            }
            else
            {
                Debug.WriteLine("[VMU931_Device] Device connected...");
            }

            while(!_firstStatusUpdateReceived)
            {
                Debug.WriteLine("[VMU931_Device] Requesting status...");
                RequestStatusUpdate();
                Thread.Sleep(100);
            }
            Debug.WriteLine("[VMU931_Device] Requesting status... done.");

            _serialConnection.ClearBuffers();

            while (_serialConnection.Connected && _connectionRunning) ;
            _serialConnection.Disconnect();
        }

        /// <summary>
        /// Enable message types that should be streamed. This function does only set a status when
        /// a first status update arrived for the device. Consider not calling this function right after connecting to the device.
        /// </summary>
        /// <param name="type"></param>
        public void EnableMessageType(MessageType type)
        {
            if(_serialConnection != null && _serialConnection.Connected && _firstStatusUpdateReceived)
            {
                if(!DeviceStatus.MessageTypeStreamingEnabled(type)) {
                    _serialConnection.SendMessageType(type);
                    RequestStatusUpdate();
                }
            }
        }

        /// <summary>
        /// Disable message types that should be streamed. This function does only set a status when
        /// a first status update arrived for the device. Consider not calling this function right after connecting to the device.
        /// </summary>
        /// <param name="type"></param>
        public void DisableMessageType(MessageType type)
        {
            if (_serialConnection != null && _serialConnection.Connected && _firstStatusUpdateReceived)
            {
                if (DeviceStatus.MessageTypeStreamingEnabled(type))
                {
                    _serialConnection.SendMessageType(type);
                    RequestStatusUpdate();
                }
            }
        }

        /// <summary>
        /// Request the current status of the device. Hook to the DeviceStatusReceived event to get this information.
        /// </summary>
        public void RequestStatusUpdate()
        {
            if (_serialConnection != null && _serialConnection.Connected)
            {
                _serialConnection.SendMessageType(MessageType.Status);
            }
        }

        private void SerialConnection_DataFrameReceived(VMU931_Frame dataFrame)
        {
            DataFrameReceived?.Invoke(dataFrame);
        }

        private void SerialConnection_StatusFrameReceived(VMU931_Status statusFrame)
        {
            _firstStatusUpdateReceived = true;
            DeviceStatus = statusFrame;
            DeviceStatusReceived?.Invoke(statusFrame);
        }
    }
}
