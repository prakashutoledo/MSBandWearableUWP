using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using IDEASLabUT.MSBandWearable.Application.Service;
using System;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 accelerometer sensor
    /// </summary>
    public class AccelerometerSensor : BaseSensorModel<AccelerometerEvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;
        public AccelerometerSensor() : base(new AccelerometerEvent())
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
            IBandSensor<IBandAccelerometerReading> accelerometer = MSBandService.Singleton.BandClient.SensorManager.Accelerometer;
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
            var subjectViewService = SubjectViewService.Singleton;
            IBandAccelerometerReading accelerometerReading = readingEventArgs.SensorReading;
            AccelerometerEvent accelerometerEvent = new AccelerometerEvent
            {
                AccelerationX = accelerometerReading.AccelerationX,
                AccelerationY = accelerometerReading.AccelerationY,
                AccelerationZ = accelerometerReading.AccelerationZ,
                AcquiredTime = DateTime.Now,
                ActualTime = accelerometerReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView.Value,
                SubjectId = subjectViewService.SubjectId.Value
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
        }
    }
}