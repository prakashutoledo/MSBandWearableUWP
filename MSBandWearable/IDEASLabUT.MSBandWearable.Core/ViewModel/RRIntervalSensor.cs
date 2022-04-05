using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    public class RRIntervalSensor : BaseSensorModel<RRIntervalEvent, IBandRRIntervalReading>
    {
        public RRIntervalSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(new RRIntervalEvent(), logger, msBandService, subjectViewService, ntpSyncService)
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
    
        protected override IBandSensor<IBandRRIntervalReading> GetBandSensor(IBandSensorManager bandSensorManager) => bandSensorManager.RRInterval;

        protected override string GetSensorName() => "ibi";

        protected override void UpdateSensorModel(IBandRRIntervalReading ibiReading)
        {
            Ibi = ibiReading.Interval;
        }
    }
}
