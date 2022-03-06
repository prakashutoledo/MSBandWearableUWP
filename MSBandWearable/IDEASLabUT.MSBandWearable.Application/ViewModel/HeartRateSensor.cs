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
    /// <summary>
    /// A sensor manager for Microsoft Band 2 heart rate sensor
    /// </summary>
    public class HeartRateSensor : BaseSensorModel<HeartRateEvent, IBandHeartRateReading>
    {
        private double maxBpm = 0;
        private double minBpm = 220;
        private HeartRateQuality heartRateStatus;

        public HeartRateSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(new HeartRateEvent(), logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        public double MaxBpm
        {
            get => maxBpm;
            set => UpdateAndNotify(ref maxBpm, value);
        }

        public double MinBpm
        {
            get => minBpm;
            set => UpdateAndNotify(ref minBpm, value);
        }

        public HeartRateQuality HeartRateStatus
        {
            get => heartRateStatus;
            set => UpdateAndNotify(ref heartRateStatus, value);
        }

        private double Bpm
        {
            set
            {
                Model.Bpm = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <summary>
        /// A task that can subscribe heart rate sensor from Microsoft Band 2
        /// </summary>
        /// <returns>An object used to await this task</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe();
            var heartRate = msBandService.BandClient.SensorManager.HeartRate;
            bool userConsent = UserConsent.Granted == heartRate.GetCurrentUserConsent() || await heartRate.RequestUserConsentAsync();
            if (!userConsent)
            {
                return;
            }

            UpdateSensorReadingChangedHandler(heartRate, HeartRateReadingChanged);
            _ = await heartRate.StartReadingsAsync();
        }

        public override void UpdateSensorReadingChangedHandler(IBandSensor<IBandHeartRateReading> heartRate, Action<IBandHeartRateReading> sensorReadingChanged)
        {
            if(heartRate == null)
            {
                return;
            }

            heartRate.ReadingChanged += (sender, readingEventArgs) =>
            {
                sensorReadingChanged.Invoke(readingEventArgs.SensorReading);
            };
        }

        /// <summary>
        /// A callback for subscribing heart rate senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">A heart rate reading event arguments</param>
        /// <see cref="BandSensorReadingEventArgs{T}"/>
        private async void HeartRateReadingChanged(IBandHeartRateReading heartRateReading)
        { 
            var heartRateEvent = new HeartRateEvent
            {
                Bpm = heartRateReading.HeartRate,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                ActualTime = heartRateReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(UpdateHeartRateValue, heartRateEvent);
            await RunLaterInUIThread(() => HeartRateStatus = heartRateReading.Quality);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(heartRateEvent);
            }

            if (subjectViewService.SessionInProgress)
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
