/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Sensor;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// An interface for managing supported sensors, see the status of conneted MS Band 2
    /// </summary>
    public interface IBandManagerService
    {
        /// <summary>
        /// The current status of the connected MS Band 2
        /// </summary>
        BandStatus BandStatus { get; }

        /// <summary>
        /// The unique name of the connected MS Band 2
        /// </summary>
        string BandName { get; }

        /// <summary>
        /// An accelerometer sensor of the connected MS Band 2
        /// </summary>
        AccelerometerSensor Accelerometer { get; }

        /// <summary>
        /// A gsr sensor of the connected MS Band 2
        /// </summary>
        GSRSensor Gsr { get; }

        /// <summary>
        /// A gyroscope sensor of the connected MS Band 2
        /// </summary>
        GyroscopeSensor Gyroscope { get; }

        /// <summary>
        /// A heart rate sensor of the connected MS Band 2
        /// </summary>
        HeartRateSensor HeartRate { get; }

        /// <summary>
        /// A temperature sensor of the connected MS Band 2
        /// </summary>
        TemperatureSensor Temperature { get; }

        /// <summary>
        /// A rr interval sensor of the connected MS Band 2
        /// </summary>
        RRIntervalSensor RRInterval { get; }

        /// Connects the given selected index from the available paired MS bands with given name
        /// </summary>
        /// <param name="bandName">A name of the band to connect</param>
        /// <returns>A task that can be awaited</returns>
        Task ConnectBand(string bandName);

        /// <summary>
        /// Subscribe all available sensors of connected MS Band 2
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        Task SubscribeSensors();


        /// <summary>
        /// Unsubscribe all available sensors of connected MS Band 2
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        Task UnsubscribeSensors();

        /// <summary>
        /// Find all the MS Band 2 which are paired using bluetooth
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        Task<IEnumerable<string>> GetPairedBands();
    }
}
