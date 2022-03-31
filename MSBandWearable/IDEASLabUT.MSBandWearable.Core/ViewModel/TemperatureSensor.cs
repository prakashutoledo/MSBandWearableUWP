using static IDEASLabUT.MSBandWearable.Core.Util.MSBandWearableCoreUtil;

using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;
using System;

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

        protected override async void SensorReadingChanged(IBandSkinTemperatureReading temperatureReading)
        {
            var temperatureEvent = new TemperatureEvent
            {
                Temperature = temperatureReading.Temperature,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                ActualTime = temperatureReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() => Temperature = temperatureEvent.Temperature);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(temperatureEvent);
            }

            if (subjectViewService.SessionInProgress)
            {
                logger.Information("{temperature}", temperatureEvent);
            }
        }
    }
}
