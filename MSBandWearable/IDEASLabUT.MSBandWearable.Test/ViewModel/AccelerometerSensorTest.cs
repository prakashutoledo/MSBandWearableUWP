﻿using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    /// <summary>
    /// View model test for <see cref="AccelerometerSensor"/>
    /// </summary>
    [TestClass]
    public class AccelerometerSensorTest : BaseSensorTest<AccelerometerEvent, IBandAccelerometerReading, AccelerometerSensor>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AccelerometerSensorTest"/>
        /// </summary>
        public AccelerometerSensorTest() : base(sensorManager => sensorManager.Accelerometer, (logger, bandClientService, subjectViewService, ntpSyncService) => new AccelerometerSensor(logger, bandClientService, subjectViewService, ntpSyncService))
        {
        }
        
        [TestMethod]
        public async Task OnAccelerometerReadingChanged()
        {
            await MockSensorReadingChanged(When(r => r.AccelerationX, 1.0), When(r => r.AccelerationY, 2.0), When(r => r.AccelerationZ, 3.0));

            var expectedModel = NewModel(value =>
            {
                value.AccelerationX = 1.0;
                value.AccelerationY = 2.0;
                value.AccelerationZ = 3.0;
            });

            VerifySensorValueChanged(expectedModel);
        }
    }
}