using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System;
using System.Threading.Tasks;
using Serilog;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    public class TemperatureSensor : BaseSensorModel<TemperatureEvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;

        public TemperatureSensor(ILogger logger) : base(new TemperatureEvent(), logger)
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
            await base.Subscribe().ConfigureAwait(false);
            var temperature = MSBandService.Singleton.BandClient.SensorManager.SkinTemperature;
            temperature.ReadingChanged += TemperatueReadingChanged;
            _ = await temperature.StartReadingsAsync().ConfigureAwait(false);
        }

        private async void TemperatueReadingChanged(object sender, BandSensorReadingEventArgs<IBandSkinTemperatureReading> readingEventArgs)
        {
            var subjectViewService = SubjectViewService.Singleton;
            var temperatureReading = readingEventArgs.SensorReading
            var temperatureEvent = new TemperatureEvent
            {
                Temperature = temperatureReading.Temperature,
                AcquiredTime = NtpSyncService.Singleton.LocalTimeNow,
                ActualTime = temperatureReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() => Temperature = temperatureEvent.Temperature);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(temperatureEvent).ConfigureAwait(false);
            }

            if (SubjectViewService.Singleton.IsSessionInProgress)
            {
                logger.Information("{temperature}", temperatureEvent);
            }
        }
    }
}
