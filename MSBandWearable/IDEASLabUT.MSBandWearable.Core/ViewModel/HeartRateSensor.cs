/*
 * Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
 */
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 heart rate sensor
    /// </summary>
    public class HeartRateSensor : BaseSensorViewModel<HeartRateEvent, IBandHeartRateReading>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HeartRateSensor"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp synchronization to set</param>
        public HeartRateSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.HeartRate, logger, msBandService, subjectViewService, ntpSyncService, sensorManager => sensorManager.HeartRate)
        {
        }

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="heartRateReading">An updated heartRate reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(in IBandHeartRateReading heartRateReading)
        {
            Model.Bpm = heartRateReading.HeartRate;
            Model.HeartRateStatus = heartRateReading.Quality;
        }
    }
}
