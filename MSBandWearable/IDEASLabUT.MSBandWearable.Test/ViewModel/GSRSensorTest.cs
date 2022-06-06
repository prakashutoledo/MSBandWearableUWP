using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
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
        public GSRSensorTest() : base(sensorManager => sensorManager.Gsr, (logger, bandClientService, subjectViewService, ntpSyncService) => new GSRSensor(logger, bandClientService, subjectViewService, ntpSyncService))
        {
        }

        [TestMethod]
        public async Task OnGSRReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.Resistance, 1000));

            var expectedModel = NewModel(value => value.Gsr = 1.0);

            VerifySensorValueChanged(expectedModel);
        }
    }
}
