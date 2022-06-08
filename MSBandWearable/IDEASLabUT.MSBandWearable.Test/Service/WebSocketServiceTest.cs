using HyperMock;

using IDEASLabUT.MSBandWearable.Model.Notification;
using IDEASLabUT.MSBandWearable.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.Networking.Sockets;

namespace IDEASLabUT.MSBandWearable.Service
{
    [TestClass]
    public class WebSocketServiceTest : AwaitableTest
    {
        private WebSocketService webSocketService;
        Mock<IUtf8MessageWebSocket> webSocketMessage;
        private Func<IUtf8MessageWebSocket> originalSocketSupplier;

        [TestInitialize]
        public void SocketSetup()
        {
            webSocketMessage = Mock.Create<IUtf8MessageWebSocket>();
            webSocketService = WebSocketService.Singleton;
            originalSocketSupplier = Utf8MessageWebSocket.SocketSupplier;
            Utf8MessageWebSocket.SocketSupplier = () => webSocketMessage.Object;
        }

        [TestMethod]
        public async Task ConnectAsync()
        {
            webSocketMessage.Setup(message => message.ConnectAsync("some-fake-url", Param.IsAny<Func<bool, Task>>())).Returns(Task.CompletedTask);
            await webSocketService.Connect("some-fake-url");
            await webSocketMessage.Object.OnMessageReceived.Invoke("12");
            webSocketMessage.Verify(message => message.ConnectAsync("some-fake-url", Param.IsAny<Func<bool, Task>>()), Occurred.Exactly(1));
        }

        [TestCleanup]
        public void SocketCleanup()
        {
            Utf8MessageWebSocket.SocketSupplier = originalSocketSupplier;
        }
    }
}
