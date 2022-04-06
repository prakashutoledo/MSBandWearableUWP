using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 temperature sensor
    /// </summary>
    public class TemperatureSensor : BaseSensorModel<TemperatureEvent, IBandSkinTemperatureReading>
    {
        public TemperatureSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.Temperature, new TemperatureEvent(), logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        private double Temperature
        {
            set
            {
                Model.Temperature = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <inheritdoc />
        protected override IBandSensor<IBandSkinTemperatureReading> GetBandSensor(IBandSensorManager sensorManager) => sensorManager.SkinTemperature;

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="temperatureReading">An updated temperature reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(IBandSkinTemperatureReading temperatureReading)
        {
            Temperature = temperatureReading.Temperature;
        }
    }
}
