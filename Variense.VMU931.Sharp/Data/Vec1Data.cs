using System;
using Variense.VMU931.Sharp.Parser;

namespace Variense.VMU931.Sharp.Data
{
    public class Vec1Data
    {
        public UInt32 Timestamp;

        public float X;

        internal void ParseData(byte[] timestamp, byte[] x)
        {
            Timestamp = ByteParser.ToUint32(timestamp);
            X = ByteParser.ToFloat(x);
        }

        public override string ToString()
        {
            return $"t:{Timestamp}, x:{X}";
        }
    }
}
