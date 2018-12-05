using System;
using Variense.VMU931.Sharp.Parser;

namespace Variense.VMU931.Sharp.Data
{
    public struct Vec4Data
    {
        public UInt32 Timestamp;

        public float X;

        public float Y;

        public float Z;

        public float W;

        internal void ParseData(byte[] timestamp, byte[] x, byte[] y, byte[] z, byte[] w)
        {
            Timestamp = ByteParser.ToUint32(timestamp);
            X = ByteParser.ToUint32(x);
            Y = ByteParser.ToFloat(y);
            Z = ByteParser.ToFloat(z);
            W = ByteParser.ToFloat(w);
        }

        public override string ToString()
        {
            return $"t:{Timestamp}, x:{X}, y:{Y}, z:{Z}, w:{W}";
        }
    }
}
