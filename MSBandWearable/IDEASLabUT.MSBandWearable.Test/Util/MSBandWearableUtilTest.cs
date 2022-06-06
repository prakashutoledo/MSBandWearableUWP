using static IDEASLabUT.MSBandWearable.Core.Util.MSBandWearableCoreUtil;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;
using IDEASLabUT.MSBandWearable.Core.Model;

namespace IDEASLabUT.MSBandWearable.Test.Util
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
            string test = "";
            await RunLaterInUIThread(() => ApplyLatch(() => test = "Fake set"));
            WaitFor();
            Assert.AreEqual("Fake set", test, $"{test} is updated in UI thread and should match actual value");
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
            Assert.AreEqual(temperatureEvent, actualEvent);
        }
    }
}
