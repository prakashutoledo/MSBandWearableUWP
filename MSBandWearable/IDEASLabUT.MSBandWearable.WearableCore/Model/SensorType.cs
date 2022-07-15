/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

using static IDEASLabUT.MSBandWearable.Model.SensorType;
using static IDEASLabUT.MSBandWearable.WearableCoreGlobals;

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
        private static readonly Lazy<IReadOnlyDictionary<string, SensorType>> sensorTypeMap;
        private static readonly Lazy<IReadOnlyDictionary<SensorType, string>> sensorNameMap;

        static SensorTypeExtension()
        {
            sensorTypeMap = new Lazy<IReadOnlyDictionary<string, SensorType>>(() =>
            {
                return new Dictionary<string, SensorType>()
                {
                    { AccelerometerSensorName, Accelerometer },
                    { GSRSensorName, GSR },
                    { GyroscopeSensorName, Gyroscope },
                    { HeartRateSensorName, HeartRate },
                    { RRIntervalSensorName, RRInterval },
                    { TemperatureSensorName, Temperature }
                };
            });

            sensorNameMap = new Lazy<IReadOnlyDictionary<SensorType, string>>(() => SensorTypeMap.ToDictionary(entry => entry.Value, entry => entry.Key));
        }

        private static IReadOnlyDictionary<string, SensorType> SensorTypeMap => sensorTypeMap.Value;

        private static IReadOnlyDictionary<SensorType, string> SensorNameMap => sensorNameMap.Value;

        /// <summary>
        /// Get name of given sensor type enum value
        /// </summary>
        /// <param name="sensorType">A sensor type enum value to return name for</param>
        /// <returns>The name of given sensor type</returns>
        public static string GetName(this SensorType sensorType)
        {
            return SensorNameMap[sensorType];
        }

        /// <summary>
        /// Gets the matching sensor type enum value for the given name
        /// </summary>
        /// <param name="name">A name of enum value to match</param>
        /// <returns>A matching nullable <see cref="SensorType?"/></returns>
        public static SensorType? ToSensorType(this string name)
        {
            return name == null ? null : SensorTypeMap.TryGetValue(name, out SensorType sensorType) ? (SensorType?) sensorType : null;
        }
    }
}
