/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Temperature sensor view model
    /// </summary>
    public class TemperatureModel : BaseViewModel
    {
        private static readonly Lazy<TemperatureModel> TemperatureModelInstance;

        static TemperatureModel()
        {
            TemperatureModelInstance = new Lazy<TemperatureModel>(() => new TemperatureModel());
        }

        internal static TemperatureModel Singleton => TemperatureModelInstance.Value;

        private double temperature;

        private TemperatureModel() : base()
        {
        }

        /// <summary>
        /// A current temperature value in celsious
        /// </summary>
        public double Temperature
        {
            get => temperature;
            set => UpdateAndNotify(ref temperature, value);
        }
    }
}
