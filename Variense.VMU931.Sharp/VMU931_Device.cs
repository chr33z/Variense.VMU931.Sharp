using System.Threading;
using Variense.VMU931.Sharp.Data;

namespace Variense.VMU931.Sharp
{
    public class VMU931_Device
    {
        public delegate void DataFrameArrivedHandler(VMU931_Frame frame);
        public DataFrameArrivedHandler DataFrameArrived;

        public int MaxRetries = 5;

        private SerialConnection _serialConnection;

        private Thread _serialThread;

        private bool _connectionRunning = false;

        /// <summary>
        /// Start a connection to the VMU931 which will immediatelly send data.
        /// Retrieve data by connecting to the <see cref="DataFrameArrived"/> event
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

        public void Disconnect()
        {
            _connectionRunning = false;
            _serialThread.Join();
        }

        private void RunSerialConnection(bool retryConnect = true, string port = "")
        {
            _connectionRunning = true;

            _serialConnection = new SerialConnection();
            _serialConnection.SerialDataReceived += SerialConnection_SerialDataReceived;

            int retries = MaxRetries;
            while (retries-- > 0)
            {
                if (_serialConnection.Connect(port)) break;
                Thread.Sleep(100);
            }

            while (_serialConnection.Connected && _connectionRunning) ;
            _serialConnection.Disconnect();
        }

        private void SerialConnection_SerialDataReceived(VMU931_Frame dataFrame)
        {
            DataFrameArrived?.Invoke(dataFrame);
        }
    }
}
