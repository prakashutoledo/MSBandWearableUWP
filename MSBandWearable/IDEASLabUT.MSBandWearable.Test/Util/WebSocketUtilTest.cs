using IDEASLabUT.MSBandWearable.Model.Notification;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static IDEASLabUT.MSBandWearable.Model.Notification.PayloadType;
using static IDEASLabUT.MSBandWearable.Util.WebSocketUtil;

namespace IDEASLabUT.MSBandWearable.Util
{
    [TestClass]
    public class WebSocketUtilTest : AwaitableTest
    {
        [TestMethod]
        public void ValidateSupportedNotificationTypeMap()
        {
            Assert.AreEqual(1, SupportedNotificationTypeMap.Count);
            Assert.AreEqual(typeof(EmpaticaE4BandMessage), SupportedNotificationTypeMap[E4Band]);
        }

        [DataTestMethod]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow("", true)]
        public async Task ShouldValidInvalidInputsDuringMessageParse(string messageToParse, bool emptyProcessors)
        {
            var status = await ParseMessageAndProcess(messageToParse, emptyProcessors ? new Dictionary<PayloadType, Func<object, Task>>() : null);
            Assert.IsFalse(status, "Parsing message was unsuccessful");
        }

        [DataTestMethod]
        [DataRow("", E4Band)]
        [DataRow("{}", E4Band)]
        [DataRow("{\"payloadType\": \"MSBand\"}", MSBand)]
        [DataRow("{\"payloadType\": \"E4Band\"}", MSBand)]
        public async Task ShouldParseMessageAsBaseMessage(string jsonMessage, PayloadType payloadType)
        {
            var processors = new Dictionary<PayloadType, Func<object, Task>>
            {
                {
                    payloadType, _ =>
                    {
                        ApplyLatch();
                        return Task.CompletedTask;
                    }
                }
            };
            var status = await ParseMessageAndProcess(jsonMessage, processors);
            WaitFor();
        }
    }
}
