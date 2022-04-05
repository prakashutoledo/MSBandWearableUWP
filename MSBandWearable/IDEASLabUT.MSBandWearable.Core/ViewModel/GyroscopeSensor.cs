using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

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

        protected override IBandSensor<IBandGyroscopeReading> GetBandSensor(IBandSensorManager bandSensorManager) => bandSensorManager.Gyroscope;

        protected override string GetSensorName() => "gyroscope";

        /// <summary>
        /// A callback for subscribing gyroscope senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">An gyroscope reading event Argument</param>
        /// <returns>A task that can be awaited</returns>
        protected override void UpdateSensorModel(IBandGyroscopeReading gyroscopeReading)
        {
            AngularX = gyroscopeReading.AngularVelocityX;
            AngularY = gyroscopeReading.AngularVelocityY;
            AngularZ = gyroscopeReading.AngularVelocityZ;
        }
    }
}
