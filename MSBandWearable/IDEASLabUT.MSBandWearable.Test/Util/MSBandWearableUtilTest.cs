﻿using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Model.Notification;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading.Tasks;

using static IDEASLabUT.MSBandWearable.Util.MSBandWearableCoreUtil;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// Unit test of utility methods
    /// </summary>
    [TestClass]
    public class MSBandWearableUtilTest : AwaitableTest
    {
        [TestMethod]
        public async Task RunLaterInMainThread()
        {
            string mutable = "";
            await RunLaterInUIThread(() => ApplyLatch(() => mutable = "Fake set"));
            WaitFor();
            Assert.AreEqual("Fake set", mutable, $"{nameof(mutable)} is updated in UI thread and should match actual value");
        }

        [TestMethod]
        public async Task RunEventUpdateInMainThread()
        {
            var temperatureEvent = new TemperatureEvent
            {
                Temperature = 1.0
            };

            var actualEvent = (TemperatureEvent) null;

            await RunLaterInUIThread(caughtEvent => ApplyLatch(() => actualEvent = caughtEvent), temperatureEvent);
            WaitFor();
            Assert.AreEqual(temperatureEvent, actualEvent, $"{nameof(actualEvent)} gets updated in ui thread and should match expected value");
        }

        [TestMethod]
        public void JsonSerialization()
        {
            var eventTime = new DateTime(2009, 8, 1, 1, 1, 1, 100);
            var gsrEvent = new GSREvent
            {
                Gsr = 1.0,
                AcquiredTime = eventTime,
                ActualTime = eventTime,
                FromView = "Fake View",
                SubjectId = "Fake Id",
            };

            var expectedJson = "{\"gsr\":1.0,\"acquiredTime\":\"2009-08-01T01:01:01.100000-0400\",\"actualTime\":\"2009-08-01T01:01:01.100000-0400\",\"bandType\":\"MSBAND\",\"fromView\":\"Fake View\",\"subjectId\":\"Fake Id\"}";
            Assert.AreEqual(expectedJson, gsrEvent.ToJson(), "Expected json should match actual json string");
        }
    }
}
