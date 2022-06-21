/*
 * Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
 */
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;

using Microsoft.Band.Sensors;

using Serilog;

using static IDEASLabUT.MSBandWearable.Model.SensorType;

namespace IDEASLabUT.MSBandWearable.Sensor
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 accelerometer sensor
    /// </summary>
    public class AccelerometerSensor : BaseSensor<AccelerometerEvent, IBandAccelerometerReading>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AccelerometerSensor"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp synchronization to set</param>
        public AccelerometerSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(Accelerometer, logger, msBandService, subjectViewService, ntpSyncService, sensorManager => sensorManager.Accelerometer)
        {
        }

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="accelerometerReading">An updated readonly refrence of accelorometer reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(in IBandAccelerometerReading accelerometerReading)
        {
            Model.AccelerationX = accelerometerReading.AccelerationX;
            Model.AccelerationY = accelerometerReading.AccelerationY;
            Model.AccelerationZ = accelerometerReading.AccelerationZ;
        }
    }
}
