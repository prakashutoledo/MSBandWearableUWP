using System;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// HeartRate sensor view model 
    /// </summary>
    public class HeartRateModel : BaseViewModel
    {
        private static readonly Lazy<HeartRateModel> HeartRateModelInstance;

        static HeartRateModel()
        {
            HeartRateModelInstance = new Lazy<HeartRateModel>(() => new HeartRateModel());
        }

        internal static HeartRateModel Singleton => HeartRateModelInstance.Value; 

        private int bpm;
        private int maxBpm = 0;
        private int minBpm = 220;
        private object heartRateStatus = false;

        private HeartRateModel() : base()
        {
        }

        /// <summary>
        /// A heart rate beats per minute value 
        /// </summary>
        public int Bpm
        {
            get => bpm;
            set => UpdateAndNotify(ref bpm, value);
        }

        /// <summary>
        /// A maximum heartrate value which also raise property changed event while setting to new values
        /// </summary>
        public int MaxBpm
        {
            get => maxBpm;
            set => UpdateAndNotify(ref maxBpm, value);
        }

        /// <summary>
        /// A minimum heartrate value which also raise property changed event while setting to new values
        /// </summary>
        public int MinBpm
        {
            get => minBpm;
            set => UpdateAndNotify(ref minBpm, value);
        }

        /// <summary>
        /// A heartrate quality value which also raise property changed event while setting to new values
        /// </summary>
        public bool HeartRateStatus
        {
            get => (bool) heartRateStatus;
            set => UpdateAndNotify(ref heartRateStatus, value);
        }
    }
}
