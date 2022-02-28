using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using IDEASLabUT.MSBandWearable.Application.Service;
using System;
using Serilog.Core;
using Serilog;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 accelerometer sensor
    /// </summary>
    public class AccelerometerSensor : BaseSensorModel<AccelerometerEvent>
    { 
        public event SensorValueChangedHandler SensorValueChanged;
        public AccelerometerSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(new AccelerometerEvent(), logger, msBandService, subjectViewService, ntpSyncService)
        {
            
        }

        private double AccelerationX
        {
            set
            {
                Model.AccelerationX = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        private double AccelerationY
        {
            set
            {
                Model.AccelerationY = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        private double AccelerationZ
        {
            set
            {
                Model.AccelerationZ = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <summary>
        /// A task that can subscribe accelerometer sensor from Microsoft Band 2
        /// </summary>
        /// <returns>An object used to await this task</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe().ConfigureAwait(false);
            var accelerometer = msBandService.BandClient.SensorManager.Accelerometer;
            accelerometer.ReadingChanged += AccelerometerReadingChanged;
            _ = await accelerometer.StartReadingsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// A callback for subscribing accelerometer senser reading event changes
        /// </summary>
        /// <param name="sender">A sender of the current changed event</param>
        /// <param name="readingEventArgs">An accelerometer reading event arguments</param>
        private async void AccelerometerReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> readingEventArgs)
        {
            var accelerometerReading = readingEventArgs.SensorReading;
            var accelerometerEvent = new AccelerometerEvent
            {
                AccelerationX = accelerometerReading.AccelerationX,
                AccelerationY = accelerometerReading.AccelerationY,
                AccelerationZ = accelerometerReading.AccelerationZ,
                ActualTime = accelerometerReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                AcquiredTime = NtpSyncService.Singleton.LocalTimeNow,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() => { AccelerationX = accelerometerEvent.AccelerationX; AccelerationY = accelerometerEvent.AccelerationY; AccelerationZ = accelerometerEvent.AccelerationZ; }).ConfigureAwait(false);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(accelerometerEvent).ConfigureAwait(false);
            }

            if (subjectViewService.IsSessionInProgress)
            {
                logger.Information("{accelerometer}", accelerometerEvent);
            }
        }
    }
}
