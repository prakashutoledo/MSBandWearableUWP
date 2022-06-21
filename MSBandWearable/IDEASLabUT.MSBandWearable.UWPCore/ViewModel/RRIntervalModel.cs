using System;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    public class RRIntervalModel : BaseViewModel
    {
        private static readonly Lazy<RRIntervalModel> RRIntervalModelInstance;

        static RRIntervalModel()
        {
            RRIntervalModelInstance = new Lazy<RRIntervalModel>(() => new RRIntervalModel());
        }

        internal static RRIntervalModel Singleton => RRIntervalModelInstance.Value;

        private double ibi;

        private RRIntervalModel() : base()
        {
        }

        /// <summary>
        /// A current inter beats interval (ibi) value
        /// </summary>
        public double Ibi
        {
            get => ibi;
            set => UpdateAndNotify(ref ibi, value);
        }
    }
}
