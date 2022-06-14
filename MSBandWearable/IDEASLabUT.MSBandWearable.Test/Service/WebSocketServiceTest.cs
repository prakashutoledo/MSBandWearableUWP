using IDEASLabUT.MSBandWearable.Model.Notification;
using IDEASLabUT.MSBandWearable.Test;
using IDEASLabUT.MSBandWearable.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage.Streams;

using static HyperMock.Occurred;
using static IDEASLabUT.MSBandWearable.Model.Notification.PayloadAction;
using static IDEASLabUT.MSBandWearable.Model.Notification.PayloadType;

namespace IDEASLabUT.MSBandWearable.Service
{
    [TestClass]
    public class WebSocketServiceTest : BaseHyperMock<WebSocketService>
    {
        private Func<IUtf8MessageWebSocket> originalSocketSupplier;
        private Func<bool, Task> continueWith;
        private bool actualStatus = false;

        [TestInitialize]
        public void SocketSetup()
        {
            originalSocketSupplier = Utf8MessageWebSocket.SocketSupplier;
            Utf8MessageWebSocket.SocketSupplier = () => MockValue<IUtf8MessageWebSocket>();
            continueWith = (status) =>
            {
                ApplyLatch(() => actualStatus = status);
                return Task.CompletedTask;
            };
        }

        [DataTestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public async Task ShouldConnectAsync(bool throwException, bool expectedStatus)
        {
            await SetupConnection("wss://some-fake-url", throwException ? Task.FromException(new Exception("fake exception")) : Task.CompletedTask);
            MockFor<IUtf8MessageWebSocket>(mockMessage => mockMessage.Verify(message => message.ConnectAsync("wss://some-fake-url"), Exactly(1)));
            Assert.AreEqual(expectedStatus, actualStatus, "WebSocket connection status");
        }

        [TestMethod]
        public async Task ShouldSendMessage()
        {
            await SetupConnection("wss://new-fake-url", Task.CompletedTask);

            IRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            var dataWriter = new DataWriter(randomAccessStream);
            MockFor<IUtf8MessageWebSocket>(mockMessage => mockMessage.SetupGet(message => message.MessageWriter).Returns(dataWriter));
            var expectedMessage = NewMessage();

            actualStatus = false;
            await Subject.SendMessage(expectedMessage, continueWith);
            WaitFor();

            randomAccessStream.Seek(0);
            using (var streamReader = new StreamReader(randomAccessStream.AsStreamForRead()))
            {
                var actualMessage = await streamReader.ReadToEndAsync();
                Assert.AreEqual(expectedMessage.ToJson(), actualMessage, "Raw json message should match");
                Assert.IsTrue(actualStatus, "Send message task is succesfull");
            }
        }

        [TestMethod]
        public void ShouldHaveMessagePostProcessor()
        {
           /*
            Assert.AreEqual(0, Subject.GetMessagePostProcessors.Count, "Post processor is empty");

            Subject.AddMessagePostProcessor<EmpaticaE4Band>(E4Band, null);
            Assert.IsFalse(Subject.GetMessagePostProcessors.ContainsKey(E4Band));

            Func<EmpaticaE4Band, Task> processor = _ => Task.CompletedTask;
            Subject.AddMessagePostProcessor(E4Band, processor);
            Assert.AreEqual(processor, Subject.GetMessagePostProcessors[E4Band]);

            Func<EmpaticaE4Band, Task> newProcessor = _ => Task.CompletedTask;
            Subject.AddMessagePostProcessor(E4Band, newProcessor);
            Assert.AreEqual(newProcessor, Subject.GetMessagePostProcessors[E4Band]);
           */
        }

        [TestCleanup]
        public void SocketCleanup()
        {
            Utf8MessageWebSocket.SocketSupplier = originalSocketSupplier;
        }

        /// <summary>
        /// Setup webSocket connection with given fakeUrl and awaitable task as result
        /// </summary>
        /// <param name="fakeUrl">A fake webSocket connection url</param>
        /// <param name="task">An awaitable task to return after connection</param>
        /// <returns>A task that can be awaited</returns>
        private async Task SetupConnection(string fakeUrl, Task task)
        {
            MockFor<IUtf8MessageWebSocket>(mockMessage => mockMessage.Setup(message => message.ConnectAsync(fakeUrl)).Returns(task));
            await Subject.Connect(fakeUrl, continueWith);
            WaitFor();
        }

        /// <summary>
        /// Creates a new instance of <see cref="EmpaticaE4BandMessage"/> with fake data
        /// </summary>
        /// <returns>A newly created empatica e4 band message</returns>
        private EmpaticaE4BandMessage NewMessage()
        {
            return new EmpaticaE4BandMessage
            {
                Payload = new EmpaticaE4Band
                {
                    Device = new Device
                    {
                        Connected = true,
                        SerialNumber = "Fake Number"
                    },
                    FromView = "Fake View",
                    SubjectId = "Fake Id"
                },
                PayloadType = E4Band,
                Action = SendMessage
            };
        }
    }
}
