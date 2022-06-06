using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;
using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    [TestClass]
    public class HeartRateSensorTest : BaseSensorTest<HeartRateEvent, IBandHeartRateReading>
    {
        public HeartRateSensorTest() : base(sensorManager => sensorManager.HeartRate, (logger, bandClientService, subjectViewService, ntpSyncService) => new HeartRateSensor(logger, bandClientService, subjectViewService, ntpSyncService))
        {
        }

        [TestMethod]
        public async Task OnHeartRateReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.HeartRate, 97));

            var expectedModel = NewModel(value => value.Bpm = 97.0);

            VerifySensorValueChanged(expectedModel);
        }
    }
}
