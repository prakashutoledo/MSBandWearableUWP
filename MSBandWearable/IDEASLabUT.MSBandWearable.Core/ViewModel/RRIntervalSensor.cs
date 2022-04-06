using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 RR Interval sensor
    /// </summary>
    public class RRIntervalSensor : BaseSensorModel<RRIntervalEvent, IBandRRIntervalReading>
    {
        public RRIntervalSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.RRInterval, new RRIntervalEvent(), logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        private double Ibi
        {
            set
            {
                Model.Ibi = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <inheritdoc />
        protected override IBandSensor<IBandRRIntervalReading> GetBandSensor(IBandSensorManager bandSensorManager) => bandSensorManager.RRInterval;

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="ibiReading">An updated RR interval reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(IBandRRIntervalReading ibiReading)
        {
            Ibi = ibiReading.Interval;
        }
    }
}
