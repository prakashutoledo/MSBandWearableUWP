﻿using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
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
        public RRIntervalSensorTest() : base(sensorManager => sensorManager.RRInterval, (logger, bandClientService, subjectViewService, ntpSyncService) => new RRIntervalSensor(logger, bandClientService, subjectViewService, ntpSyncService))
        {
        }

        [TestMethod]
        public async Task OnGSRReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.Interval, 50.0));

            var expectedModel = NewModel(value => value.Ibi = 50.0);

            VerifySensorValueChanged(expectedModel);
        }
    }
}
