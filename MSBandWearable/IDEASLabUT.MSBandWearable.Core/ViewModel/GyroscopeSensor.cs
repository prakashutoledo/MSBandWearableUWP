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
        public GyroscopeSensor(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService) : base(SensorType.Gyroscope, new GyroscopeEvent(), logger, msBandService, subjectViewService, ntpSyncService)
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

        /// <inheritdoc />
        protected override IBandSensor<IBandGyroscopeReading> GetBandSensor(IBandSensorManager bandSensorManager) => bandSensorManager.Gyroscope;

        /// <summary>
        /// Updates the underlying model value
        /// </summary>
        /// <param name="gyroscopeReading">An updated gyroscope reading value to be reflected to model changed</param>
        protected override void UpdateSensorModel(IBandGyroscopeReading gyroscopeReading)
        {
            AngularX = gyroscopeReading.AngularVelocityX;
            AngularY = gyroscopeReading.AngularVelocityY;
            AngularZ = gyroscopeReading.AngularVelocityZ;
        }
    }
}
