using System;
using System.Threading;
using VMU931_Sharp.Data;

namespace VMU931_Sharp_ConsoleTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            VMU931_Sharp.VMU931_Device _vmu931_device = new VMU931_Sharp.VMU931_Device();

            _vmu931_device.DataFrameArrived += VMU932_DataFrameArrived;
            _vmu931_device.Connect(true);

            int i = 0;
            while (i < 30)
            {
                Console.WriteLine($"{i++} I'm counting... and counting...");
                Thread.Sleep(250);
            }

            _vmu931_device.Disconnect();
        }

        private static void VMU932_DataFrameArrived(VMU931_Frame frame)
        {
            Console.WriteLine(frame.ToString());
        }
    }
}
