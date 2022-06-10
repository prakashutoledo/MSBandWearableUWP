using System;
using System.Threading.Tasks;

using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// A message web socket which handles utf8 string webSocket message
    /// </summary>
    public class Utf8MessageWebSocket : IUtf8MessageWebSocket
    {
        /// <summary>
        /// Supplies the new instance of <see cref="IUtf8MessageWebSocket"/>
        /// </summary>
        public static Func<IUtf8MessageWebSocket> SocketSupplier = () => new Utf8MessageWebSocket();

        private readonly MessageWebSocket messageWebSocket;

        /// <summary>
        /// Creates a new instance of <see cref="Utf8MessageWebSocket"/>
        /// </summary>
        private Utf8MessageWebSocket()
        {
            // Private initialization
            messageWebSocket = new MessageWebSocket();
            MessageWriter = new DataWriter(messageWebSocket.OutputStream);
        }

        /// <summary>
        /// A message received callback which will be called once websocket message is received
        /// </summary>
        public Func<string, Task> OnMessageReceived { get; set; }

        /// <summary>
        /// A websocket message writer to send utf-8 string message
        /// </summary>
        public IDataWriter MessageWriter { get; }

        /// <summary>
        /// Connects to given webSocketUrl asynchronously
        /// </summary>
        /// <param name="webSocketUrl">A webSocket url to connect</param>
        /// <returns>A task that can be awaited</returns>
        public async Task ConnectAsync(string webSocketUrl)
        {
            messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
            messageWebSocket.MessageReceived += MessageReceivedEvent;
            await messageWebSocket.ConnectAsync(new Uri(webSocketUrl));
        }

        /// <summary>
        /// A callback for receiving webSocket message
        /// </summary>
        /// <param name="sender">A sender of this message</param>
        /// <param name="receivedEventArgs">A messsage websocket message received event arguments</param>
        private async void MessageReceivedEvent(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs receivedEventArgs)
        {
            if (receivedEventArgs.MessageType != SocketMessageType.Utf8)
            {
                // This class doesn't support binary webSocket message
                return;
            }

            using (var dataReader = receivedEventArgs.GetDataReader())
            {
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);

                if (OnMessageReceived != null)
                {
                    await OnMessageReceived.Invoke(message);
                }
            }
        }

        /// <summary>
        /// Close the underlying webSocket instance with given code and reason
        /// </summary>
        /// <param name="code">A code to set for closing webSocket</param>
        /// <param name="reason">A reason phrase for closing</param>
        public void Close(ushort code, string reason)
        {
            messageWebSocket?.Close(code, reason);
        }

        /// <summary>
        /// Dispose the resources used by underlying message webSocket instance
        /// </summary>
        public void Dispose()
        {
            _ = (MessageWriter?.DetachStream());
            messageWebSocket?.Dispose();
        }
    }
}
