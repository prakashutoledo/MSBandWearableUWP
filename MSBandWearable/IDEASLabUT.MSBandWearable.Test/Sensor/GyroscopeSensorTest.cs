/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Sensor
{
    /// <summary>
    /// View model test for <see cref="GyroscopeSensor"/>
    /// </summary>
    [TestClass]
    public class GyroscopeSensorTest : BaseSensorTest<GyroscopeEvent, IBandGyroscopeReading, GyroscopeSensor>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GyroscopeSensorTest"/>
        /// </summary>
        public GyroscopeSensorTest() : base(sensorManager => sensorManager.Gyroscope)
        {
        }

        [TestMethod]
        public async Task OnGyroscopeReadingChanged()
        {
            await MockSensorReadingChanged(
                When(reading => reading.AngularVelocityX, 1.0),
                When(reading => reading.AngularVelocityY, 2.0),
                When(reading => reading.AngularVelocityZ, 3.0)
            );

            var expectedModel = NewModel(value => 
            {
                value.AngularX = 1.0;
                value.AngularY = 2.0;
                value.AngularZ = 3.0;
            });

            VerifySensorValueChanged(expectedModel);
        }
    }
}
