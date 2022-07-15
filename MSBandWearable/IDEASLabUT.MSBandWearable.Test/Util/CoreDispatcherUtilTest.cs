/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

using static IDEASLabUT.MSBandWearable.Util.CoreDispatcherUtil;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// Unit test of utility methods
    /// </summary>
    [TestClass]
    public class CoreDispatcherUtilTest : AwaitableTest
    {
        
        [TestMethod]
        public async Task RunLaterInMainThread()
        {
            string mutable = "";
            await RunLaterInUIThread(() => ApplyLatch(() => mutable = "Fake set"));
            await WaitAsync();
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
            await WaitAsync();
            Assert.AreEqual(temperatureEvent, actualEvent, $"{nameof(actualEvent)} gets updated in ui thread and should match expected value");
        }
    }
}
