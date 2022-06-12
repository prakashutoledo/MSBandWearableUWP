using IDEASLabUT.MSBandWearable.Model;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// View model test for <see cref="RRIntervalSensor"/>
    /// </summary>
    [TestClass]
    public class RRIntervalSensorTest : BaseSensorTest<RRIntervalEvent, IBandRRIntervalReading, RRIntervalSensor>
    {
        /// <summary>
        /// Creates a new instance of <see cref="RRIntervalSensorTest"/>
        /// </summary>
        public RRIntervalSensorTest() : base(sensorManager => sensorManager.RRInterval)
        {
        }

        [TestMethod]
        public async Task OnGSRReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.Interval, 50.0))
            VerifySensorValueChanged(NewModel(value => value.Ibi = 50.0));
        }
    }
}
