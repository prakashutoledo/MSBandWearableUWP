using IDEASLabUT.MSBandWearable.Model;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// View model test for <see cref="TemperatureSensor"/>
    /// </summary>
    [TestClass]
    public class TemperatureSensorTest : BaseSensorTest<TemperatureEvent, IBandSkinTemperatureReading, TemperatureSensor>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TemperatureSensorTest"/>
        /// </summary>
        public TemperatureSensorTest() : base(sensorManager => sensorManager.SkinTemperature)
        {
        }

        [TestMethod]
        public async Task OnGSRReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.Temperature, 37.0));
            VerifySensorValueChanged(NewModel(value => value.Temperature = 37.0));
        }
    }
}
