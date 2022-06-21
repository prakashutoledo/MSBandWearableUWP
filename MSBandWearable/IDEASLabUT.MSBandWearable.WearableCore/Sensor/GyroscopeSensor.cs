/*
 * Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
 */
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Sensor
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 gyroscope sensor
    /// </summary>
    public class GyroscopeSensor : BaseSensor<GyroscopeEvent, IBandGyroscopeReading>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GyroscopeSensor"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp synchronization to set</param>
        public GyroscopeSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.Gyroscope, logger, msBandService, subjectViewService, ntpSyncService, sensorManager => sensorManager.Gyroscope)
        {
        }

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="gyroscopeReading">An updated gyroscope reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(in IBandGyroscopeReading gyroscopeReading)
        {
            Model.AngularX = gyroscopeReading.AngularVelocityX;
            Model.AngularY = gyroscopeReading.AngularVelocityY;
            Model.AngularZ = gyroscopeReading.AngularVelocityZ;
        }
    }
}
