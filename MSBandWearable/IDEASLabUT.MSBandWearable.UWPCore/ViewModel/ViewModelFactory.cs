/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Factory class for all view model classes
    /// </summary>
    public abstract class ViewModelFactory
    {
        /// <summary>
        /// Private final implementation class of <see cref="ViewModelFactory"/>
        /// </summary>
        private sealed class ViewModelFactoryImpl : ViewModelFactory
        {
            /// <summary>
            /// Creates a new instance of <see cref="ViewModelFactory"/>
            /// </summary>
            public ViewModelFactoryImpl()
            {
                GetHeartRateModel = HeartRateModel.Singleton;
                GetGSRModel = GSRModel.Singleton;
                GetSubjectViewModel = SubjectViewModel.Singleton;
                GetRRIntervalModel = RRIntervalModel.Singleton;
                GetTemperatureModel = TemperatureModel.Singleton;
            }

            /// <summary>
            /// Gets the implementation instance of <see cref="HeartRateModel"/>
            /// </summary>
            public sealed override HeartRateModel GetHeartRateModel { get; }

            /// <summary>
            /// Gets the implementation instance of <see cref="GSRModel"/>
            /// </summary>
            public sealed override GSRModel GetGSRModel { get; }

            /// <summary>
            /// Gets the implementation instance of <see cref="SubjectViewModel"/>
            /// </summary>
            public sealed override SubjectViewModel GetSubjectViewModel { get; }

            /// <summary>
            /// Gets the implementation instance of <see cref="RRIntervalModel"/>
            /// </summary>
            public sealed override RRIntervalModel GetRRIntervalModel { get; }

            /// <summary>
            /// Gets the implementation instance of <see cref="TemperatureModel"/>
            /// </summary>
            public sealed override TemperatureModel GetTemperatureModel { get; }
        }

        private static readonly Lazy<ViewModelFactory> ViewModeFactoryInstance;

        static ViewModelFactory()
        {
            ViewModeFactoryInstance = new Lazy<ViewModelFactory>(new ViewModelFactoryImpl());
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="ViewModelFactory"/>
        /// </summary>
        public static ViewModelFactory Singleton => ViewModeFactoryInstance.Value;

        /// <summary>
        /// Gets the current <see cref="HeartRateModel"/>
        /// </summary>
        public abstract HeartRateModel GetHeartRateModel { get; }

        /// <summary>
        /// Gets the current <see cref="GSRModel"/>
        /// </summary>
        public abstract GSRModel GetGSRModel { get; }

        /// <summary>
        /// Gets the current <see cref="SubjectViewModel"/>
        /// </summary>
        public abstract SubjectViewModel GetSubjectViewModel { get; }

        /// <summary>
        /// Gets the current <see cref="RRIntervalModel"/>
        /// </summary>
        public abstract RRIntervalModel GetRRIntervalModel { get; }

        /// <summary>
        /// Gets the current <see cref="TemperatureModel"/>
        /// </summary>
        public abstract TemperatureModel GetTemperatureModel { get; }
    }
}
