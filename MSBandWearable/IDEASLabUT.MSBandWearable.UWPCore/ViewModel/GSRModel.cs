/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Gsr sensor view model
    /// </summary>
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
