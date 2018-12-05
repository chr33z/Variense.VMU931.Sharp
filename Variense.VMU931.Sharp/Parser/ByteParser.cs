using System;

namespace Variense.VMU931.Sharp.Parser
{
    internal static class ByteParser
    {
        internal static UInt32 ToUint32(byte[] data)
        {
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        internal static float ToFloat(byte[] data)
        {
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }
    }
}
