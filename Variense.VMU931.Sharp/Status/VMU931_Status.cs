using System.Collections.Generic;
using Variense.VMU931.Sharp.Data;

namespace Variense.VMU931.Sharp.Status
{
    /// <summary>
    /// Data frame containing all data gathered from one frame provided by the VMU931.
    /// 
    /// Different message types contain different kind of data:
    /// Accerometer, EulerAngles, Magnetometer: Vec3Data
    /// Quaternion: Vec4Data
    /// Heading: Vec1Data
    /// </summary>
    public struct VMU931_Status
    {
        public GyroResolution GyroscopeResolution;

        public AccelerometerResolution AccelerometerResolution;

        public OutputRate OutputRate;

        public bool AccelerometerEnabled;

        public bool GyroscopeEnabled;

        public bool MagnetometerEnabled;

        public bool AccelerometerStreamingEnabled;

        public bool GyroscopeStreamingEnabled;

        public bool MagnetometerStreamingEnabled;

        public bool EulerAngleStreamingEnabled;

        public bool QuaternionStreamingEnabled;

        public bool HeadingStreamingEnabled;

        public bool MessageTypeStreamingEnabled(MessageType type)
        {
            switch(type)
            {
                case MessageType.Accelerometer:
                    return AccelerometerStreamingEnabled;
                case MessageType.Gyroscope:
                    return GyroscopeStreamingEnabled;
                case MessageType.Magnetometers:
                    return MagnetometerStreamingEnabled;
                case MessageType.Heading:
                    return HeadingStreamingEnabled;
                case MessageType.Quaternions:
                    return QuaternionStreamingEnabled;
                case MessageType.EulerAngles:
                    return EulerAngleStreamingEnabled;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return "Device Status:\n" +
                $"GyroRes: {GyroscopeResolution}; AccRes: {AccelerometerResolution} \n" +
                $"Output Rate: {OutputRate} \n" +
                $"GyroSensor: {GyroscopeEnabled}; AccSensor: {AccelerometerEnabled}; MagSensor: {MagnetometerEnabled} \n" +
                $"GyroStream: {GyroscopeStreamingEnabled}; AccStream: {AccelerometerStreamingEnabled}; MagStream: {MagnetometerStreamingEnabled} \n" +
                $"EulerStream: {EulerAngleStreamingEnabled}; QuatStream: {QuaternionStreamingEnabled}; HeadingStream: {HeadingStreamingEnabled}";
        }
    }
}
