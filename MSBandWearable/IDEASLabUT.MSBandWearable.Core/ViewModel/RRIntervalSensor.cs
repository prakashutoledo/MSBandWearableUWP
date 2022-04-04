using static IDEASLabUT.MSBandWearable.Core.Util.MSBandWearableCoreUtil;

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

        protected override async void SensorReadingChanged(IBandRRIntervalReading ibiReading)
        {
            var ibiEvent = new RRIntervalEvent
            {
                Ibi = ibiReading.Interval,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                ActualTime = ibiReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() =>
            {
                Ibi = ibiEvent.Ibi;
            });

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(ibiEvent);
            }

            if (subjectViewService.SessionInProgress)
            {
                logger.Information("{ibi}", ibiEvent);
            }
        }
    }
}
