using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using Serilog;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    public class GSRSensor : BaseSensorModel<GSREvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;

        public GSRSensor(ILogger logger, MSBandService msBandService, SubjectViewService subjectViewService, NtpSyncService ntpSyncService) : base(new GSREvent(), logger, msBandService, subjectViewService, ntpSyncService)
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

        /// <summary>
        /// A task that can subscribe GSR sensor from Microsoft Band 2
        /// </summary>
        /// <returns>An object used to await this task</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe().ConfigureAwait(false);
            var gsr = msBandService.BandClient.SensorManager.Gsr;
            gsr.ReadingChanged += GsrReadingChanged;
            _ = await gsr.StartReadingsAsync();
        }

        private async void GsrReadingChanged(object sender, BandSensorReadingEventArgs<IBandGsrReading> readingEventArgs)
        {
            var gsrReading = readingEventArgs.SensorReading;
            var gsrEvent = new GSREvent
            {
                // Value is in kOhms which is converted into micro seimens
                Gsr = 1000.0 / gsrReading.Resistance,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                ActualTime = gsrReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() => Gsr = gsrEvent.Gsr).ConfigureAwait(false);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(gsrEvent).ConfigureAwait(false);
            }


            if (subjectViewService.IsSessionInProgress)
            {
                logger.Information("{gsr}", gsrEvent);
            }
        }
    }
}