using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    public class TemperatureSensor : BaseSensorModel<TemperatureEvent, IBandSkinTemperatureReading>
    {
        public TemperatureSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(new TemperatureEvent(), logger, msBandService, subjectViewService, ntpSyncService)
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

        protected override IBandSensor<IBandSkinTemperatureReading> GetBandSensor(IBandSensorManager sensorManager) => sensorManager.SkinTemperature;
        
        protected override string GetSensorName() => "temperature";

        protected override void UpdateSensorModel(IBandSkinTemperatureReading temperatureReading)
        {
            Temperature = temperatureReading.Temperature;
        }
    }
}
