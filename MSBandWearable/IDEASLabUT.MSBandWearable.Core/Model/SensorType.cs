using static IDEASLabUT.MSBandWearable.Util.MSBandWearableCoreUtil;

namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// Currently supported available MS Band 2 sensor types
    /// </summary>
    public enum SensorType
    {
        Accelerometer,
        GSR,
        Gyroscope,
        HeartRate,
        RRInterval,
        Temperature
    }

    /// <summary>
    /// Sensor type extension to get the name of sensors
    /// </summary>
    public static class SensorTypeExtension
    {
        /// <summary>
        /// Get name of given sensor type enum value
        /// </summary>
        /// <param name="sensorType">A sensor type enum value to return name for</param>
        /// <returns>The name of given sensor type</returns>
        public static string GetName(this SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.Accelerometer:
                    return Accelerometer;
                case SensorType.GSR:
                    return GSR;
                case SensorType.Gyroscope:
                    return Gyroscope;
                case SensorType.HeartRate:
                    return HeartRate;
                case SensorType.RRInterval:
                    return RRInterval;
                case SensorType.Temperature:
                    return Temperature;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the matching sensor type enum value for the given name
        /// </summary>
        /// <param name="name">A name of enum value to match</param>
        /// <returns>A matching nullable <see cref="SensorType?"/></returns>
        public static SensorType? FromName(string name)
        {
            if (name == null)
            {
                return null;
            }

            switch (name)
            {
                case Accelerometer:
                    return SensorType.Accelerometer;
                case GSR:
                    return SensorType.GSR;
                case Gyroscope:
                    return SensorType.Gyroscope;
                case HeartRate:
                    return SensorType.HeartRate;
                case RRInterval:
                    return SensorType.RRInterval;
                case Temperature:
                    return SensorType.Temperature;
                default:
                    return null;
            }
        }
    }
}
