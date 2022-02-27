using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using Serilog;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 gyroscope sensor
    /// </summary>
    public class GyroscopeSensor : BaseSensorModel<GyroscopeEvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;
        public GyroscopeSensor(ILogger logger, MSBandService msBandService, SubjectViewService subjectViewService, NtpSyncService ntpSyncService) : base(new GyroscopeEvent(), logger, msBandService, subjectViewService, ntpSyncService)
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
            var gyroscopeReading = readingEventArgs.SensorReading;
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

            await RunLaterInUIThread(() => { AngularX = gyroscopeEvent.AngularX; AngularY = gyroscopeEvent.AngularY; AngularZ = gyroscopeEvent.AngularZ; }).ConfigureAwait(false);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(gyroscopeEvent).ConfigureAwait(false);
            }

            if (subjectViewService.IsSessionInProgress)
            {
                logger.Information("{gyroscope}", gyroscopeEvent);
            }
        }
    }
}
