using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using Serilog;
using System;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 gyroscope sensor
    /// </summary>
    public class GyroscopeSensor : BaseSensorModel<GyroscopeEvent, IBandGyroscopeReading>
    {
        public GyroscopeSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(new GyroscopeEvent(), logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        private double AngularX
        {
            set
            {
                Model.AngularX = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        private double AngularY
        {
            set
            {
                Model.AngularX = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        private double AngularZ
        {
            set
            {
                Model.AngularX = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <summary>
        /// A task that can subscribe gyroscope sensor from Microsoft Band 2
        /// </summary>
        /// <returns>A task that is already complete</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe().ConfigureAwait(false);
            var gyroscope = msBandService.BandClient.SensorManager.Gyroscope;
            UpdateSensorReadingChangedHandler(gyroscope, GyroscopeReadingChanged);
            _ = await gyroscope.StartReadingsAsync();
        }

        public override void UpdateSensorReadingChangedHandler(IBandSensor<IBandGyroscopeReading> gyroscope, Action<IBandGyroscopeReading> sensorReadingChanged)
        {
            if(gyroscope == null) {
                return;
            }

            gyroscope.ReadingChanged += (sender, readingEventArgs) =>
            {
                sensorReadingChanged.Invoke(readingEventArgs.SensorReading);
            };
        }

        /// <summary>
        /// A callback for subscribing gyroscope senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">An gyroscope reading event Argument</param>
        private async void GyroscopeReadingChanged(IBandGyroscopeReading gyroscopeReading)
        {
            var gyroscopeEvent = new GyroscopeEvent
            {
                AngularX = gyroscopeReading.AccelerationX,
                AngularY = gyroscopeReading.AccelerationY,
                AngularZ = gyroscopeReading.AccelerationZ,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                ActualTime = gyroscopeReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() =>
            {
                AngularX = gyroscopeEvent.AngularX;
                AngularY = gyroscopeEvent.AngularY;
                AngularZ = gyroscopeEvent.AngularZ;
            });

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(gyroscopeEvent);
            }

            if (subjectViewService.SessionInProgress)
            {
                logger.Information("{gyroscope}", gyroscopeEvent);
            }
        }
    }
}
