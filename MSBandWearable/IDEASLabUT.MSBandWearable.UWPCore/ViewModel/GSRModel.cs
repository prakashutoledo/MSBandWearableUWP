using System;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    public class GSRModel : BaseViewModel
    {
        private static readonly Lazy<GSRModel> GSRModelInstance;

        static GSRModel()
        {
            GSRModelInstance = new Lazy<GSRModel>(() => new GSRModel());
        }

        internal static GSRModel Singleton => GSRModelInstance.Value;

        private GSRModel() : base()
        {
        }

        private double gsr;

        /// <summary>
        /// A current gsr resistance value in KOHMs
        /// </summary>
        public double Gsr
        {
            get => gsr;
            set => UpdateAndNotify(ref gsr, value);
        }
    }
}
