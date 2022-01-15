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
        /// <summary>
        /// A task that can subscribe accelerometer sensor from Microsoft Band 2
        /// </summary>
        /// <returns>A task that is already complete</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe().ConfigureAwait(false);
            MSBandService.Singleton.BandClient.SensorManager.Accelerometer.ReadingChanged += AccelerometerRedingChanged;
        }

        /// <summary>
        /// A callback for subscribing accelerometer senser reading event changes
        /// </summary>
        /// <param name="sender">A sender of the current changed event</param>
        /// <param name="readingEventArgs">A accelerometer reading event Argument</param>
        private async void AccelerometerRedingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> readingEventArgs)
        {
            IBandAccelerometerReading accelerometerReading = readingEventArgs.SensorReading;
            await RunLaterInUIThread(() =>
            {
                AccelerometerEvent accelerometerEvent = new AccelerometerEvent
                {
                    AccelerationX = accelerometerReading.AccelerationX,
                    AccelerationY = accelerometerReading.AccelerationY,
                    AccelerationZ = accelerometerReading.AccelerationZ
                };
                Data = accelerometerEvent;
                SensorValueChanged?.Invoke(accelerometerEvent);
            });
        }
    }
}
