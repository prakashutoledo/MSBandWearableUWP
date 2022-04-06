namespace IDEASLabUT.MSBandWearable.Core.Model
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
        /// Get name of given sensor type
        /// </summary>
        /// <param name="sensorType">A sensor type to return name for</param>
        /// <returns>The mane of given sensor type</returns>
        public static string GetName(this SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.Accelerometer:
                    return "accelerometer";
                case SensorType.GSR:
                    return "gsr";
                case SensorType.Gyroscope:
                    return "gyroscope";
                case SensorType.HeartRate:
                    return "heartrate";
                case SensorType.RRInterval:
                    return "ibi";
                case SensorType.Temperature:
                    return "temperature";
                default:
                    return null;
            }
        }
    }
}
