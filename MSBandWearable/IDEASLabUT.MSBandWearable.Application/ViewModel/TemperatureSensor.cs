using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using Serilog;
using System;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
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

        /// <summary>
        /// A task that can subscribe GSR sensor from Microsoft Band 2
        /// </summary>
        /// <returns>An object used to await this task</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe();
            var temperature = msBandService.BandClient.SensorManager.SkinTemperature;
            UpdateSensorReadingChangedHandler(temperature, TemperatueReadingChanged);
            _ = await temperature.StartReadingsAsync();
        }

        public override void UpdateSensorReadingChangedHandler(IBandSensor<IBandSkinTemperatureReading> temperature, Action<IBandSkinTemperatureReading> sensorReadingChanged)
        {
            if (temperature == null)
            {
                return;
            }

            temperature.ReadingChanged += (sender, readingEventArgs) =>
            {
                sensorReadingChanged.Invoke(readingEventArgs.SensorReading);
            };
        }

        private async void TemperatueReadingChanged(IBandSkinTemperatureReading temperatureReading)
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
