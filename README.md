# Variense.VMU931.Sharp
C# Library to gather data send by a Variense VMU931 9-DOF tracker

### Usage

```
[STAThread]
static void Main(string[] args)
{
    VMU931_Device _vmu931_device = new VMU931_Device();

    _vmu931_device.DataFrameArrived += VMU931_DataFrameArrived;
    _vmu931_device.Connect(true);

    // do something 
    int i = 0;
    while (i < 30)
    {
        Console.WriteLine($"{i++} I'm counting... and counting...");
        Thread.Sleep(250);
    }

    _vmu931_device.Disconnect();
}

private static void VMU931_DataFrameArrived(VMU931_Frame frame)
{
    Console.WriteLine(frame.ToString());
}
```

### Still to to...

- [x] ~~Implement commands to change the data that is send~~
- [x] ~~Implement status requests~~
- [ ] Implement Frequencies
