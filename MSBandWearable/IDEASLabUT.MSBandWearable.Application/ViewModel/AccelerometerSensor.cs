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
    public class AccelerometerSensor : BaseSensorModel<AccelerometerEvent, IBandAccelerometerReading>
    { 
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
        /// Updates the given accelerometer sensor to include accelerometer reading value changed handler for given action 
        /// </summary>
        /// <param name="accelerometer">A MS Band accelerometer sensor</param>
        /// <param name="sensorReadingChanged">A reading changed action for handling acceleorometer value reading</param>
        public override void UpdateSensorReadingChangedHandler(IBandSensor<IBandAccelerometerReading> accelerometer, Action<IBandAccelerometerReading> sensorReadingChanged)
        {
            accelerometer.ReadingChanged += (sender, readingEventArgs) =>
            {
                sensorReadingChanged.Invoke(readingEventArgs.SensorReading);
            };
        }

        protected override IBandSensor<IBandAccelerometerReading> GetBandSensor(IBandSensorManager bandSensorManager)
        {
            return bandSensorManager.Accelerometer;
        }

        /// <summary>
        /// A callback for subscribing accelerometer senser reading event changes
        /// </summary>
        /// <param name="sender">A sender of the current changed event</param>
        /// <param name="readingEventArgs">An accelerometer reading event arguments</param>
        protected override async void SensorReadingChanged(IBandAccelerometerReading accelerometerReading)
        {
            var accelerometerEvent = new AccelerometerEvent
            {
                AccelerationX = accelerometerReading.AccelerationX,
                AccelerationY = accelerometerReading.AccelerationY,
                AccelerationZ = accelerometerReading.AccelerationZ,
                ActualTime = accelerometerReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                AcquiredTime = ntpSyncService.LocalTimeNow,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() =>
            {
                AccelerationX = accelerometerEvent.AccelerationX;
                AccelerationY = accelerometerEvent.AccelerationY;
                AccelerationZ = accelerometerEvent.AccelerationZ;
            });

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(accelerometerEvent);
            }

            if (subjectViewService.SessionInProgress)
            {
                logger.Information("{accelerometer}", accelerometerEvent);
            }
        }
    }
}
