using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using Microsoft.Band;
using Serilog;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 heart rate sensor
    /// </summary>
    public class HeartRateSensor : BaseSensorModel<HeartRateEvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;

        public HeartRateSensor(ILogger logger, MSBandService msBandService, SubjectViewService subjectViewService, NtpSyncService ntpSyncService) : base(new HeartRateEvent(), logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        private double maxBpm = 0;
        public double MaxBpm
        {
            get => maxBpm;
            set => UpdateAndNotify(ref maxBpm, value);
        }

        public double minBpm = 220;
        public double MinBpm
        {
            get => minBpm;
            set => UpdateAndNotify(ref minBpm, value);
        }

        private double Bpm
        {
            set
            {
                Model.Bpm = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        public HeartRateQuality heartRateStatus;
        public HeartRateQuality HeartRateStatus
        {
            get => heartRateStatus;
            set => UpdateAndNotify(ref heartRateStatus, value);
        }

        /// <summary>
        /// A task that can subscribe heart rate sensor from Microsoft Band 2
        /// </summary>
        /// <returns>An object used to await this task</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe().ConfigureAwait(false);
            var heartRate = msBandService.BandClient.SensorManager.HeartRate;
            bool userConsent = UserConsent.Granted == heartRate.GetCurrentUserConsent() || await heartRate.RequestUserConsentAsync().ConfigureAwait(false);
            if (!userConsent)
            {
                return;
            }
           
            heartRate.ReadingChanged += HeartRateReadingChanged;
            _ = await heartRate.StartReadingsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// A callback for subscribing heart rate senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">A heart rate reading event arguments</param>
        /// <see cref="BandSensorReadingEventArgs{T}"/>
        private async void HeartRateReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> readingEventArgs)
        {
            var heartRateReading = readingEventArgs.SensorReading;
            var heartRateEvent = new HeartRateEvent
            {
                Bpm = heartRateReading.HeartRate,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                ActualTime = heartRateReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(UpdateHeartRateValue, heartRateEvent).ConfigureAwait(false);
            await RunLaterInUIThread(() => HeartRateStatus = heartRateReading.Quality).ConfigureAwait(false);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(heartRateEvent).ConfigureAwait(false);
            }


            if (subjectViewService.IsSessionInProgress)
            {
                logger.Information("{heartrate}", heartRateEvent);
            }
        }

        private void UpdateHeartRateValue(HeartRateEvent heartRateEvent)
        {
            Bpm = heartRateEvent.Bpm;

            if (heartRateEvent.Bpm > MaxBpm)
            {
                MaxBpm = heartRateEvent.Bpm;
                return;
            }

            if (heartRateEvent.Bpm < MinBpm)
            {
                MinBpm = heartRateEvent.Bpm;
            }
        }
    }
}