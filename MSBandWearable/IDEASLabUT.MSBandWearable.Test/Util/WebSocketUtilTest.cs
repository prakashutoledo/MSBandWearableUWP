/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model.Notification;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static IDEASLabUT.MSBandWearable.Model.Notification.PayloadType;
using static IDEASLabUT.MSBandWearable.Util.WebSocketUtil;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// Unit test for <see cref="WebSocketUtil"/>
    /// </summary>
    [TestClass]
    public class WebSocketUtilTest
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
        [DataRow("{}", E4Band)]
        [DataRow("{\"payloadType\": \"MSBand\"}", MSBand)]
        [DataRow("{\"payloadType\": \"E4Band\"}", MSBand)]
        public async Task ShouldFailedToParseAndProcess(string jsonMessage, PayloadType payloadType)
        {
            var processors = new Dictionary<PayloadType, Func<object, Task>>
            {
                { payloadType, _ => Task.CompletedTask }
            };
            var status = await ParseMessageAndProcess(jsonMessage, processors);
            Assert.IsFalse(status, "Message parsing was unsuccessfull");
        }

        [TestMethod]
        public async Task ShouldParseAndProcess()
        {
            EmpaticaE4Band receivedPayload = null;
            Task Receive(EmpaticaE4Band message)
            {
                receivedPayload = message;
                return Task.CompletedTask;
            }

            var processors = new Dictionary<PayloadType, Func<object, Task>>
            {
                {
                    E4Band, processedMessage => 
                    {
                        return Receive((processedMessage as Message<EmpaticaE4Band>).Payload);
                    }
                }
            };

            var jsonMessage = "{\"payload\":{\"payloadType\":\"E4Band\",\"subjectId\":\"Fake Subject Id\",\"fromView\":\"Fake View\",\"device\":{\"serialNumber\":\"Fake Serial Number\",\"connected\":true}},\"action\":\"sendMessage\",\"payloadType\":\"E4Band\"}";
            var status = await ParseMessageAndProcess(jsonMessage, processors);
   
            Assert.IsTrue(status, "Message is successfully parsed and processed");
            var expectedPayload = new EmpaticaE4Band
            {
                Device = new Device
                {
                    Connected = true,
                    SerialNumber = "Fake Serial Number"
                },
                FromView = "Fake View",
                SubjectId = "Fake Subject Id",
            };
            AssertPayload(expectedPayload, receivedPayload);
        }

        private void AssertPayload(EmpaticaE4Band expected, EmpaticaE4Band actual)
        {
            Assert.AreEqual(expected.PayloadType, actual.PayloadType, "Payload type should match");
            Assert.AreEqual(expected.FromView, actual.FromView, "From View should match");
            Assert.AreEqual(expected.SubjectId, actual.SubjectId, "Subject id should match");
            AssertDevice(expected.Device, actual.Device);
        }

        private void AssertDevice(Device expected, Device actual)
        {
            Assert.AreEqual(expected.Connected, actual.Connected, "Connected status should match");
            Assert.AreEqual(expected.SerialNumber, actual.SerialNumber, "Serial number should match");
        }
    }
}
