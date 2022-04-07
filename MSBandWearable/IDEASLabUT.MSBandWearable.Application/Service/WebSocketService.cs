using IDEASLabUT.MSBandWearable.Core.Model.Notification;

using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    /// <summary>
    /// A service class for managing webSocket connection details using <see cref="MessageWebSocket"/>
    /// </summary>
    public class WebSocketService
    {
        private static readonly Lazy<WebSocketService> Instance = new Lazy<WebSocketService>(() => new WebSocketService());

        // Lazy singleton pattern
        public static WebSocketService Singleton => Instance.Value;

        private Func<EmpaticaE4Band, Task> onEmpaticaE4BandMessageReceived;
        private MessageWebSocket messageWebSocket;

        /// <summary>
        /// Initializes a new instance of <see cref="WebSocketService"/>
        /// </summary>
        private WebSocketService()
        {
        }

        /// <summary>
        /// Connect to given webSocket url by setting given webSocket message received callback function
        /// </summary>
        /// <param name="webSocketUrl">A webSocket URL to connect</param>
        /// <param name="onEmpaticaE4BandMessageReceived">An asynchronous webSocket message received callback function</param>
        /// <returns>A task that can be awaited</returns>
        public async Task Connect(string webSocketUrl, Func<EmpaticaE4Band, Task> onEmpaticaE4BandMessageReceived)
        {
            messageWebSocket = new MessageWebSocket();
            if(onEmpaticaE4BandMessageReceived != null)
            {
                this.onEmpaticaE4BandMessageReceived = onEmpaticaE4BandMessageReceived;
            }
            messageWebSocket.MessageReceived += MessageReceivedEvent;
            messageWebSocket.Control.MessageType = SocketMessageType.Utf8;

            await messageWebSocket.ConnectAsync(new Uri(webSocketUrl));
        }

        /// <summary>
        /// Sends the given string message as webSocket message
        /// </summary>
        /// <param name="message">A message to be sent</param>
        /// <returns>A task that can be awaited</returns>
        public async Task SendMessage(string message)
        {
            using (var dataWriter = new DataWriter(messageWebSocket.OutputStream))
            {
                _ = dataWriter.WriteString(message);
                _ = await dataWriter.StoreAsync();
                _ = dataWriter.DetachStream();
            }
        }

        /// <summary>
        /// A callback function for receiving webSocket message
        /// </summary>
        /// <param name="sender">The sender of current message received event</param>
        /// <param name="receivedEventArgs">A message websocket message received event arguments</param>
        private async void MessageReceivedEvent(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs receivedEventArgs)
        {
            using (var dataReader = receivedEventArgs.GetDataReader())
            {
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                await ParseMessageAndSend(message);
            }
        }

        /// <summary>
        /// Serialize given webSocket json message and call the message received function
        /// </summary>
        /// <param name="message">A webSocket json message to be serialized</param>
        /// <returns>A task that can be awaited</returns>
        private async Task ParseMessageAndSend(string message)
        {
            if (message == null)
            {
                return;
            }

            var baseMessage = JsonConvert.DeserializeObject<BaseMessage>(message);
            if (baseMessage == null)
            {
                return;
            }

            switch (baseMessage.PayloadType)
            {
                case PayloadType.E4Band:
                    var empaticaE4BandMessage = JsonConvert.DeserializeObject<EmpaticaE4BandMessage>(message);
                    if (onEmpaticaE4BandMessageReceived != null)
                    {
                        await onEmpaticaE4BandMessageReceived.Invoke(empaticaE4BandMessage.Payload);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Close the given message WebSocket instance with given reason
        /// </summary>
        /// <param name="reason"></param>
        public void Close(string reason = "Application Closed")
        {
            if (messageWebSocket != null)
            {
                messageWebSocket.Close(1000, reason);
            }
        }
    }
}
