using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band.Sensors;

using Serilog;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// A sensor manager for Microsoft Band 2 accelerometer sensor
    /// </summary>
    public class AccelerometerSensor : BaseSensorViewModel<AccelerometerEvent, IBandAccelerometerReading>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AccelerometerSensor"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp synchronization to set</param>
        public AccelerometerSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.Accelerometer, new AccelerometerEvent(), logger, msBandService, subjectViewService, ntpSyncService)
        {
        }

        /// <summary>
        /// Sets the acceleration X value and raise property change event
        /// </summary>
        private double AccelerationX
        {
            set
            {
                Model.AccelerationX = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <summary>
        /// Sets the acceleration Y value and raise property change event
        /// </summary>
        private double AccelerationY
        {
            set
            {
                Model.AccelerationY = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <summary>
        /// Sets the acceleration Z value and raise property change event
        /// </summary>
        private double AccelerationZ
        {
            set
            {
                Model.AccelerationZ = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <inheritdoc />
        protected override IBandSensor<IBandAccelerometerReading> GetBandSensor(IBandSensorManager bandSensorManager) => bandSensorManager.Accelerometer;

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="accelerometerReading">An updated accelorometer reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(IBandAccelerometerReading accelerometerReading)
        {
            AccelerationX = accelerometerReading.AccelerationX;
            AccelerationY = accelerometerReading.AccelerationY;
            AccelerationZ = accelerometerReading.AccelerationZ;
        }
    }
}
