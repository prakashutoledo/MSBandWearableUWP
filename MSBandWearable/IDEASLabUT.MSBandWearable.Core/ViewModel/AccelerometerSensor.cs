using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
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

        protected override IBandSensor<IBandAccelerometerReading> GetBandSensor(IBandSensorManager bandSensorManager) => bandSensorManager.Accelerometer;

        protected override string GetSensorName() => "accelerometer";
        

        /// <summary>
        /// A callback for subscribing accelerometer senser reading event changes
        /// </summary>
        /// <param name="sender">A sender of the current changed event</param>
        /// <param name="readingEventArgs">An accelerometer reading event arguments</param>
        protected override void UpdateSensorModel(IBandAccelerometerReading accelerometerReading)
        {
            AccelerationX = accelerometerReading.AccelerationX;
            AccelerationY = accelerometerReading.AccelerationY;
            AccelerationZ = accelerometerReading.AccelerationZ;
        }
    }
}
