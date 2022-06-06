using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 heart rate sensor
    /// </summary>
    public class HeartRateSensor : BaseSensorViewModel<HeartRateEvent, IBandHeartRateReading>
    {
        private double maxBpm = 0;
        private double minBpm = 220;
        private HeartRateQuality heartRateStatus;

        /// <summary>
        /// Initializes a new instance of <see cref="HeartRateSensor"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp synchronization to set</param>
        public HeartRateSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.HeartRate, logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        /// <summary>
        /// A maximum heartrate value which also raise property changed event while setting to new values
        /// </summary>
        public double MaxBpm
        {
            get => maxBpm;
            set => UpdateAndNotify(ref maxBpm, value);
        }

        /// <summary>
        /// A minimum heartrate value which also raise property changed event while setting to new values
        /// </summary>
        public double MinBpm
        {
            get => minBpm;
            set => UpdateAndNotify(ref minBpm, value);
        }

        /// <summary>
        /// A heartrate quality value which also raise property changed event while setting to new values
        /// </summary>
        public HeartRateQuality HeartRateStatus
        {
            get => heartRateStatus;
            set => UpdateAndNotify(ref heartRateStatus, value);
        }

        /// <inheritdoc />
        protected override IBandSensor<IBandHeartRateReading> GetBandSensor(IBandSensorManager sensorManager) => sensorManager.HeartRate;

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="heartRateReading">An updated heartRate reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(IBandHeartRateReading heartRateReading)
        {
            var bpm = Model.Bpm = heartRateReading.HeartRate;
            HeartRateStatus = heartRateReading.Quality;

            if (bpm > MaxBpm)
            {
                MaxBpm = bpm;
            }

            if (bpm < MinBpm)
            {
                MinBpm = bpm;
            }
        }
    }
}
