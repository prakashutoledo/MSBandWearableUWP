using Microsoft.Band.Sensors;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    public class HeartRateModel : BaseViewModel
    {
        private double maxBpm = 0;
        private double minBpm = 220;
        private HeartRateQuality heartRateStatus;


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
    }
}
