﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
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

        [TestInitialize]
        public void SetupGsr()
        {
            MockFor<IBandSensor<IBandGsrReading>>(
                gsrMock => gsrMock.SetupGet(gsr => gsr.SupportedReportingIntervals).Returns(new List<TimeSpan> {
                    TimeSpan.FromMilliseconds(200)
                })
            );
        }

        [TestMethod]
        public async Task OnGSRReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.Resistance, 1000));
            VerifySensorValueChanged(NewModel(value => value.Gsr = 1.0));
        }
    }
}
