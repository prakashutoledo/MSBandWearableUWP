using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 gyroscope sensor
    /// </summary>
    public class GyroscopeSensor : BaseSensorModel<GyroscopeEvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;
        public GyroscopeSensor() : base(new GyroscopeEvent())
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
            IBandSensor<IBandGyroscopeReading> gyroscope = MSBandService.Singleton.BandClient.SensorManager.Gyroscope;
            gyroscope.ReadingChanged += GyroscopeReadingChanged;
            _ = await gyroscope.StartReadingsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// A callback for subscribing gyroscope senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">An gyroscope reading event Argument</param>
        private async void GyroscopeReadingChanged(object sender, BandSensorReadingEventArgs<IBandGyroscopeReading> readingEventArgs)
        {
            SubjectViewService subjectViewService = SubjectViewService.Singleton;
            IBandGyroscopeReading gyroscopeReading = readingEventArgs.SensorReading;

            GyroscopeEvent gyroscopeEvent = new GyroscopeEvent
            {
                AngularX = gyroscopeReading.AccelerationX,
                AngularY = gyroscopeReading.AccelerationY,
                AngularZ = gyroscopeReading.AccelerationZ,
                AcquiredTime = NtpSyncService.Singleton.LocalTimeNow,
                ActualTime = gyroscopeReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView.Value,
                SubjectId = subjectViewService.SubjectId.Value
            };

            await RunLaterInUIThread(() => { AngularX = gyroscopeEvent.AngularX; AngularY = gyroscopeEvent.AngularY; AngularZ = gyroscopeEvent.AngularZ; }).ConfigureAwait(false);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(gyroscopeEvent).ConfigureAwait(false);
            }


            if (SubjectViewService.Singleton.IsSessionInProgress.Value)
            {

            }
        }
    }
}
