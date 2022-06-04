using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 temperature sensor
    /// </summary>
    public class TemperatureSensor : BaseSensorViewModel<TemperatureEvent, IBandSkinTemperatureReading>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TemperatureSensor"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp synchronization to set</param>
        public TemperatureSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.Temperature, logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        /// <inheritdoc />
        protected override IBandSensor<IBandSkinTemperatureReading> GetBandSensor(IBandSensorManager sensorManager) => sensorManager.SkinTemperature;

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="temperatureReading">An updated temperature reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(IBandSkinTemperatureReading temperatureReading)
        {
            Model.Temperature = temperatureReading.Temperature;
        }
    }
}
