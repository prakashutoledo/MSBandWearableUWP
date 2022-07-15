/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Sensor
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 RR Interval sensor
    /// </summary>
    public class RRIntervalSensor : BaseSensor<RRIntervalEvent, IBandRRIntervalReading>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RRIntervalSensor"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp synchronization to set</param>
        public RRIntervalSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.RRInterval, logger, msBandService, subjectViewService, ntpSyncService, sensorManager => sensorManager.RRInterval)
        {
        }

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="ibiReading">An updated RR interval reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(in IBandRRIntervalReading ibiReading)
        {
            Model.Ibi = ibiReading.Interval;
        }
    }
}
