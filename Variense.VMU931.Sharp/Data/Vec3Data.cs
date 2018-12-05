using System;
using Variense.VMU931.Sharp.Parser;

namespace Variense.VMU931.Sharp.Data
{
    public struct Vec3Data
    {
        public UInt32 Timestamp;

        public float X;

        public float Y;

        public float Z;

        internal void ParseData(byte[] timestamp, byte[] x, byte[] y, byte[] z)
        {
            Timestamp = ByteParser.ToUint32(timestamp);
            X = ByteParser.ToFloat(x);
            Y = ByteParser.ToFloat(y);
            Z = ByteParser.ToFloat(z);
        }

        public override string ToString()
        {
            return $"t:{Timestamp}, x:{X}, y:{Y}, z:{Z}";
        }
    }
}
