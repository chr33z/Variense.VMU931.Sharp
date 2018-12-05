namespace Variense.VMU931.Sharp.Parser
{
    internal static class CommandType
    {
        internal const char Accelerometer = 'a';
        internal const char Gyroscope = 'g';
        internal const char EulerAngles = 'e';
        internal const char Quaternions = 'q';
        internal const char Heading = 'h';
        internal const char Magnetometers = 'c';
        internal const char Status = 's';

        internal static bool IsCommandType(char c)
        {
            return
                c == Accelerometer ||
                c == Gyroscope ||
                c == EulerAngles ||
                c == Quaternions ||
                c == Heading ||
                c == Magnetometers ||
                c == Status;
        }
    }
}
