using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System;
using System.Threading.Tasks;
using Microsoft.Band;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 heart rate sensor
    /// </summary>
    public class HeartRateSensor : BaseSensorModel<HeartRateEvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;

        public HeartRateSensor() : base(new HeartRateEvent())
        {
        }

        private double maxBpm;
        public double MaxBpm
        {
            get => maxBpm;
            set => UpdateAndNotify(ref maxBpm, value);
        }

        public double minBpm;
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

        private bool IsFirstBpmValue { get; set; } = true;

        /// <summary>
        /// A task that can subscribe heart rate sensor from Microsoft Band 2
        /// </summary>
        /// <returns>An object used to await this task</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe().ConfigureAwait(false);
            var heartRate = MSBandService.Singleton.BandClient.SensorManager.HeartRate;
            bool requestHeartRateUserConsent = false;

            if (heartRate.GetCurrentUserConsent() == UserConsent.Granted)
            {
                requestHeartRateUserConsent = true;
            }
            else
            {
                requestHeartRateUserConsent = await heartRate.RequestUserConsentAsync();
            }

            if (!requestHeartRateUserConsent)
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
            var subjectViewService = SubjectViewService.Singleton;
            var heartRateReading = readingEventArgs.SensorReading;
            var heartRateEvent = new HeartRateEvent
            {
                Bpm = heartRateReading.HeartRate,
                AcquiredTime = DateTime.Now,
                ActualTime = heartRateReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView.Value,
                SubjectId = subjectViewService.SubjectId.Value
            };


            await RunLaterInUIThread(() =>
            {
                Bpm = heartRateEvent.Bpm;
                if (IsFirstBpmValue)
                {
                    MaxBpm = heartRateEvent.Bpm;
                    MinBpm = heartRateEvent.Bpm;
                    IsFirstBpmValue = false;
                    return;
                }

                if (heartRateEvent.Bpm > MaxBpm)
                {
                    MaxBpm = heartRateEvent.Bpm;
                    return;
                }

                if (heartRateEvent.Bpm >= MinBpm)
                {
                    return;
                }
                MinBpm = heartRateEvent.Bpm;
            });

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(heartRateEvent);
            }
        }
    }
}
