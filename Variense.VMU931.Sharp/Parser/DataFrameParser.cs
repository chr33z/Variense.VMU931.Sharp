using Variense.VMU931.Sharp.Data;
using Variense.VMU931.Sharp.Status;
using Variense.VMU931.Sharp.Utils;

namespace Variense.VMU931.Sharp.Parser
{
    /// <summary>
    /// Parsing according to:
    /// https://variense.com/Docs/VMU931/VMU931_UserGuide.pdf
    /// 
    /// </summary>
    internal class DataFrameParser
    {
        internal const byte MessageStart = 0x01;

        internal const byte MessageEnd = 0x04;

        internal static VMU931_Status ParseStatusFrame(byte[] frame)
        {
            VMU931_Status status = new VMU931_Status();

            if (frame.Length != 7) return status;

            var sensors = frame[0];
            var resolutions = frame[1];
            var outputRate = frame[2];
            var messageTypes = frame[6];

            if ((sensors & (1 << 0)) != 0) status.AccelerometerEnabled = true;
            if ((sensors & (1 << 1)) != 0) status.GyroscopeEnabled = true;
            if ((sensors & (1 << 2)) != 0) status.MagnetometerEnabled = true;

            for (int i = 0; i < 8; i++)
            {
                if (i < 4)
                {
                    // accelerometer resolution
                    if ((resolutions & (1 << i)) != 0) status.AccelerometerResolution = (AccelerometerResolution)i;
                }
                else
                {
                    // gyroresolution
                    if ((resolutions & (1 << i)) != 0) status.AccelerometerResolution = (AccelerometerResolution)i + 4;
                }
            }

            if ((outputRate & (1 << 0)) != 0) status.OutputRate = OutputRate.Low;
            else status.OutputRate = OutputRate.Low;

            if ((messageTypes & (1 << 0)) != 0) status.AccelerometerStreamingEnabled = true;
            if ((messageTypes & (1 << 1)) != 0) status.GyroscopeStreamingEnabled = true;
            if ((messageTypes & (1 << 2)) != 0) status.QuaternionStreamingEnabled = true;
            if ((messageTypes & (1 << 3)) != 0) status.MagnetometerStreamingEnabled = true;
            if ((messageTypes & (1 << 4)) != 0) status.EulerAngleStreamingEnabled = true;
            if ((messageTypes & (1 << 5)) != 0) status.HeadingStreamingEnabled = true;

            return status;
        }

        internal static VMU931_Frame ParseDateFrame(byte[] frame, char messageType)
        {

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
