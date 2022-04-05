using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    public class GSRSensor : BaseSensorModel<GSREvent, IBandGsrReading>
    {
        public GSRSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(new GSREvent(), logger, msBandService, subjectViewService, ntpSyncService)
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

        protected override IBandSensor<IBandGsrReading> GetBandSensor(IBandSensorManager bandSensorManager) => bandSensorManager.Gsr;

        protected override string GetSensorName() => "gsr";

        protected override void UpdateSensorModel(IBandGsrReading gsrReading)
        {
            Gsr = 1000.0 / gsrReading.Resistance;
        }
    }
}
