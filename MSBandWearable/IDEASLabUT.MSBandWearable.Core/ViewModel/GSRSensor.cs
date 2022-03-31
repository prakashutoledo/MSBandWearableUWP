using static IDEASLabUT.MSBandWearable.Core.Util.MSBandWearableCoreUtil;

using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;
using System;

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

        protected override IBandSensor<IBandGsrReading> GetBandSensor(IBandSensorManager bandSensorManager)
        {
            return bandSensorManager.Gsr;
        }

        protected override async void SensorReadingChanged(IBandGsrReading gsrReading)
        {
            var gsrEvent = new GSREvent
            {
                // Value is in kOhms which is converted into micro seimens
                Gsr = 1000.0 / gsrReading.Resistance,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                ActualTime = gsrReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() => Gsr = gsrEvent.Gsr);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(gsrEvent);
            }

            if (subjectViewService.SessionInProgress)
            {
                logger.Information("{gsr}", gsrEvent);
            }
        }
    }
}
