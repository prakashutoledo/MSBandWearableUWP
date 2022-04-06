using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 GSR sensor
    /// </summary>
    public class GSRSensor : BaseSensorModel<GSREvent, IBandGsrReading>
    {
        public GSRSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.GSR, new GSREvent(), logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        private double Gsr
        {
            set
            {
                Model.Gsr = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <inheritdoc />
        protected override IBandSensor<IBandGsrReading> GetBandSensor(IBandSensorManager bandSensorManager) => bandSensorManager.Gsr;

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="gsrReading">An updated gsr reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(IBandGsrReading gsrReading)
        {
            Gsr = 1000.0 / gsrReading.Resistance;
        }
    }
}
