using static IDEASLabUT.MSBandWearable.Core.Util.MSBandWearableCoreUtil;

using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;
using System;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
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

        public override void UpdateSensorReadingChangedHandler(IBandSensor<IBandGyroscopeReading> gyroscope, Action<IBandGyroscopeReading> sensorReadingChanged)
        {
            gyroscope.ReadingChanged += (sender, readingEventArgs) =>
            {
                sensorReadingChanged.Invoke(readingEventArgs.SensorReading);
            };
        }

        protected override IBandSensor<IBandGyroscopeReading> GetBandSensor(IBandSensorManager bandSensorManager)
        {
            return bandSensorManager.Gyroscope;
        }

        /// <summary>
        /// A callback for subscribing gyroscope senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">An gyroscope reading event Argument</param>
        protected override async void SensorReadingChanged(IBandGyroscopeReading gyroscopeReading)
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
