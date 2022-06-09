using HyperMock;

using IDEASLabUT.MSBandWearable.Model.Notification;
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
    public class WebSocketServiceTest : AwaitableTest
    {
        private WebSocketService webSocketService;
        private Mock<IUtf8MessageWebSocket> webSocketMessage;
        private Func<IUtf8MessageWebSocket> originalSocketSupplier;
        private Func<bool, Task> continueWith;
        private bool actualStatus = false;

        [TestInitialize]
        public void SocketSetup()
        {
            webSocketMessage = Mock.Create<IUtf8MessageWebSocket>();
            webSocketService = WebSocketService.Singleton;
            originalSocketSupplier = Utf8MessageWebSocket.SocketSupplier;
            Utf8MessageWebSocket.SocketSupplier = () => webSocketMessage.Object;
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
            webSocketMessage.Verify(message => message.ConnectAsync("wss://some-fake-url"), Exactly(1));
            Assert.AreEqual(expectedStatus, actualStatus, "WebSocket connection status");
        }

        [TestMethod]
        public async Task ShouldSendMessage()
        {
            await SetupConnection("wss://new-fake-url", Task.CompletedTask);

            IRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            var dataWriter = new DataWriter(randomAccessStream);
            webSocketMessage.SetupGet(webSocketMessage => webSocketMessage.DataWriter).Returns(dataWriter);
            var message = NewMessage();

            actualStatus = false;
            await webSocketService.SendMessage(message, continueWith);
            WaitFor();

            randomAccessStream.Seek(0);
            using (var streamReader = new StreamReader(randomAccessStream.AsStreamForRead()))
            {
                var actualMessage = await streamReader.ReadToEndAsync();
                Assert.AreEqual(message.ToJson(), actualMessage, "Raw json message should match");
                Assert.IsTrue(actualStatus, "Send message task is succesfull");
            }
        }

        [TestMethod]
        public void ShouldHaveMessagePostProcessor()
        {
            webSocketService.SetMessagePostProcessor(E4Band, null);
            Assert.IsFalse(webSocketService.GetMessagePostProcessors.ContainsKey(E4Band));

            Func<object, Task> processor = _ => Task.CompletedTask;
            webSocketService.SetMessagePostProcessor(E4Band, processor);
            Assert.AreEqual(processor, webSocketService.GetMessagePostProcessors[E4Band]);
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
            webSocketMessage.Setup(webSocketMessage => webSocketMessage.ConnectAsync(fakeUrl)).Returns(task);
            await webSocketService.Connect(fakeUrl, continueWith);
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
