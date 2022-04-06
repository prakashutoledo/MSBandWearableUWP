using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
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

        /// <inheritdoc />
        protected override IBandSensor<IBandHeartRateReading> GetBandSensor(IBandSensorManager sensorManager) => sensorManager.HeartRate;

        /// <inheritdoc />
        protected override string GetSensorName() => "heartrate";

        /// <summary>
        /// A callback for subscribing heart rate senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">A heart rate reading event arguments</param>

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="heartRateReading">An updated heartRate reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(IBandHeartRateReading heartRateReading)
        {
            Bpm = heartRateReading.HeartRate;
            HeartRateStatus = heartRateReading.Quality;

            if (Model.Bpm > MaxBpm)
            {
                MaxBpm = Model.Bpm;
                return;
            }

            if (Model.Bpm < MinBpm)
            {
                MinBpm = Model.Bpm;
            }
        }
    }
}
