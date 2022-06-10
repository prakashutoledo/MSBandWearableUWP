using IDEASLabUT.MSBandWearable.Model;
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
    }
}
