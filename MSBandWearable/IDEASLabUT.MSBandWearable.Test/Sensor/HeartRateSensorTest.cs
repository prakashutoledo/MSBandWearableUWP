/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

using static Microsoft.Band.Sensors.HeartRateQuality;

namespace IDEASLabUT.MSBandWearable.Sensor
{
    /// <summary>
    /// View model test for <see cref="HeartRateSensor"/>
    /// </summary>
    [TestClass]
    public class HeartRateSensorTest : BaseSensorTest<HeartRateEvent, IBandHeartRateReading, HeartRateSensor>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeartRateSensorTest"/>
        /// </summary>
        public HeartRateSensorTest() : base(sensorManager => sensorManager.HeartRate)
        {
        }

        [TestMethod]
        public async Task OnHeartRateReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.HeartRate, 97), When(reading => reading.Quality, Locked));

            var expectedModel = NewModel(value => 
            {
                value.Bpm = 97;
                value.HeartRateStatus = Locked;
            });
            VerifySensorValueChanged(expectedModel: expectedModel);

        }
    }
}
