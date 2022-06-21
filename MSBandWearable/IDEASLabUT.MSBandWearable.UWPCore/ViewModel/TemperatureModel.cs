using System;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
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
