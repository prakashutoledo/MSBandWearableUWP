/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;

using Microsoft.Band.Sensors;

using Serilog;

using System.Linq;
namespace IDEASLabUT.MSBandWearable.Sensor
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 GSR sensor
    /// </summary>
    public class GSRSensor : BaseSensor<GSREvent, IBandGsrReading>
    {
        public GSRSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.GSR, logger, msBandService, subjectViewService, ntpSyncService, GetGsrSensor)
        {
        }

        private static IBandSensor<IBandGsrReading> GetGsrSensor(IBandSensorManager bandSensorManager)
        {
            var gsr = bandSensorManager.Gsr;
            if (gsr != null)
            {
                gsr.ReportingInterval = gsr.SupportedReportingIntervals.Last();
            }
            return gsr;
        }



        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="gsrReading">An updated gsr reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(in IBandGsrReading gsrReading)
        {
            Model.Gsr = 1000.0 / gsrReading.Resistance; // Resistance is in KOhms, need to converted into microseimens
        }
    }
}
