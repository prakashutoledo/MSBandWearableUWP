using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using IDEASLabUT.MSBandWearable.Application.Service;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    public class AccelerometerSensor : BaseSensorModel<AccelerometerEvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;
        public AccelerometerSensor() : base(new AccelerometerEvent())
        {
            
        }

        public double AccelerationX
        {
            get => Model.AccelerationX;
            set
            {
                Model.AccelerationX = value;
                NotifyPropertyChanged();
            }
        }

        public double AccelerationY
        {
            get => Model.AccelerationY;
            set
            {
                Model.AccelerationY = value;
                NotifyPropertyChanged();
            }
        }

        public double AccelerationZ
        {
            get => Model.AccelerationZ;
            set
            {
                Model.AccelerationZ = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// A task that can subscribe accelerometer sensor from Microsoft Band 2
        /// </summary>
        /// <returns>A task that is already complete</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe().ConfigureAwait(false);
            MSBandService.Singleton.BandClient.SensorManager.Accelerometer.ReadingChanged += AccelerometerReadingChanged;
        }

        /// <summary>
        /// A callback for subscribing accelerometer senser reading event changes
        /// </summary>
        /// <param name="sender">A sender of the current changed event</param>
        /// <param name="readingEventArgs">A accelerometer reading event Argument</param>
        private async void AccelerometerReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> readingEventArgs)
        {
            IBandAccelerometerReading accelerometerReading = readingEventArgs.SensorReading;
            await RunLaterInUIThread(() =>
            {
                AccelerationX = accelerometerReading.AccelerationX;
                AccelerationY = accelerometerReading.AccelerationY;
                AccelerationZ = accelerometerReading.AccelerationZ;
                SensorValueChanged?.Invoke(Model);
            });
        }
    }
}
