using System;
using Variense.VMU931.Sharp.Data;
using Variense.VMU931.Sharp.Utils;

namespace Variense.VMU931.Sharp.Parser
{
    internal class DataFrameParser
    {
        internal const byte MessageStart = 0x01;

        internal const byte MessageEnd = 0x04;

        internal static VMU931_Frame ParseDateFrame(byte[] frame, char messageType) {

            Vec1Data vec1data;
            Vec3Data vec3data;
            Vec4Data vec4data;

            switch (messageType)
            {
                case CommandType.EulerAngles:
                    vec3data = ParseVec3Data(frame);
                    return new VMU931_Frame()
                    {
                        Vec3Data = vec3data,
                        MessageType = MessageType.EulerAngles
                    };
                case CommandType.Accelerometer:
                    vec3data = ParseVec3Data(frame);
                    return new VMU931_Frame()
                    {
                        Vec3Data = vec3data,
                        MessageType = MessageType.Accelerometer
                    };
                case CommandType.Magnetometers:
                    vec3data = ParseVec3Data(frame);
                    return new VMU931_Frame()
                    {
                        Vec3Data = vec3data,
                        MessageType = MessageType.Magnetometers
                    };
                case CommandType.Quaternions:
                    vec4data = ParseVec4Data(frame);
                    return new VMU931_Frame()
                    {
                        Vec4Data = vec4data,
                        MessageType = MessageType.Quaternions
                    };
                case CommandType.Heading:
                    vec1data = ParseVec1Data(frame);
                    return new VMU931_Frame()
                    {
                        Vec1Data = vec1data,
                        MessageType = MessageType.Heading
                    };
            }

            return new VMU931_Frame();
        }

        private static Vec1Data ParseVec1Data(byte[] frame)
        {
            var data = new Vec1Data();
            data.ParseData(
                ArrayUtilities.RangeSubset(frame, 0, 4),
                ArrayUtilities.RangeSubset(frame, 4, 4)
                );

            return data;
        }

        private static Vec3Data ParseVec3Data(byte[] frame)
        {
            var data = new Vec3Data();
            data.ParseData(
                ArrayUtilities.RangeSubset(frame, 0, 4),
                ArrayUtilities.RangeSubset(frame, 4, 4),
                ArrayUtilities.RangeSubset(frame, 8, 4),
                ArrayUtilities.RangeSubset(frame, 12, 4)
                );

            return data;
        }

        private static Vec4Data ParseVec4Data(byte[] frame)
        {
            var data = new Vec4Data();
            data.ParseData(
                ArrayUtilities.RangeSubset(frame, 0, 4),
                ArrayUtilities.RangeSubset(frame, 4, 4),
                ArrayUtilities.RangeSubset(frame, 8, 4),
                ArrayUtilities.RangeSubset(frame, 12, 4),
                ArrayUtilities.RangeSubset(frame, 16, 4)
                );

            return data;
        }
    }
}
