﻿using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
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
        public GyroscopeSensorTest() : base(sensorManager => sensorManager.Gyroscope, (logger, bandClientService, subjectViewService, ntpSyncService) => new GyroscopeSensor(logger, bandClientService, subjectViewService, ntpSyncService))
        {
        }

        [TestMethod]
        public async Task OnGyroscopeReadingChanged()
        {
            await MockSensorReadingChanged(When(r => r.AngularVelocityX, 1.0), When(r => r.AngularVelocityY, 2.0), When(r => r.AngularVelocityZ, 3.0));

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