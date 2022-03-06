using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using Microsoft.Band;
using Serilog;
using System;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
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

        /// <summary>
        /// A task that can subscribe GSR sensor from Microsoft Band 2
        /// </summary>
        /// <returns>An object used to await this task</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe();
            var ibi = msBandService.BandClient.SensorManager.RRInterval;

            bool userConsent = UserConsent.Granted == ibi.GetCurrentUserConsent() || await ibi.RequestUserConsentAsync();
            if (!userConsent)
            {
                return;
            }

            UpdateSensorReadingChangedHandler(ibi, RRIntervalReadingChanged);
            _ = await ibi.StartReadingsAsync();
        }

        public override void UpdateSensorReadingChangedHandler(IBandSensor<IBandRRIntervalReading> ibi, Action<IBandRRIntervalReading> sensorReadingChanged)
        {
            if (ibi == null)
            {
                return;
            }

            ibi.ReadingChanged += (sender, readingEventArgs) =>
            {
                sensorReadingChanged.Invoke(readingEventArgs.SensorReading);
            };
        }

        private async void RRIntervalReadingChanged(IBandRRIntervalReading ibiReading)
        {
            var ibiEvent = new RRIntervalEvent
            {
                Ibi = ibiReading.Interval,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                ActualTime = ibiReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() => Ibi = ibiEvent.Ibi);

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
