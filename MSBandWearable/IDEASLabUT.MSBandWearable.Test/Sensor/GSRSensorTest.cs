using IDEASLabUT.MSBandWearable.Model;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Sensor
{
    /// <summary>
    /// View model test for <see cref="GSRSensor"/>
    /// </summary>
    [TestClass]
    public class GSRSensorTest : BaseSensorTest<GSREvent, IBandGsrReading, GSRSensor>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GSRSensorTest"/>
        /// </summary>
        public GSRSensorTest() : base(sensorManager => sensorManager.Gsr)
        {
        }

        [TestMethod]
        public async Task OnGSRReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.Resistance, 1000));
            VerifySensorValueChanged(NewModel(value => value.Gsr = 1.0));
        }
    }
}
