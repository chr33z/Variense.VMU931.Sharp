namespace VMU931_Sharp.Data
{
    /// <summary>
    /// Data frame containing all data gathered from one frame provided by the VMU931.
    /// 
    /// Different message types contain different kind of data:
    /// Accerometer, EulerAngles, Magnetometer: Vec3Data
    /// Quaternion: Vec4Data
    /// Heading: Vec1Data
    /// </summary>
    public struct VMU931_Frame
    {
        public MessageType MessageType;

        public Vec1Data Vec1Data;

        public Vec3Data Vec3Data;

        public Vec4Data Vec4Data;

        public override string ToString()
        {
            switch (MessageType)
            {
                case MessageType.Accelerometer:
                case MessageType.EulerAngles:
                case MessageType.Magnetometers:
                    return $"{MessageType} {Vec3Data.ToString()}";
                case MessageType.Quaternions:
                    return $"{MessageType} {Vec4Data.ToString()}";
                case MessageType.Heading:
                    return $"{MessageType} {Vec1Data.ToString()}";
                default:
                    return $"{MessageType}";
            }
        }
    }
}
