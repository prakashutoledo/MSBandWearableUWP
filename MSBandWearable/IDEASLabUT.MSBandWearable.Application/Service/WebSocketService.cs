using IDEASLabUT.MSBandWearable.Core.Model.Notification;

using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class WebSocketService
    {
        private static readonly Lazy<WebSocketService> Instance = new Lazy<WebSocketService>(() => new WebSocketService());

        // Lazy singleton pattern
        public static WebSocketService Singleton => Instance.Value;

        private Func<EmpaticaE4Band, Task> onEmpaticaE4BandMessageReceived;
        private MessageWebSocket messageWebSocket;

        private WebSocketService()
        {
        }

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


        public async Task SendMessage(string message)
        {
            using (var dataWriter = new DataWriter(messageWebSocket.OutputStream))
            {
                _ = dataWriter.WriteString(message);
                _ = await dataWriter.StoreAsync();
                _ = dataWriter.DetachStream();
            }
        }

        public async void MessageReceivedEvent(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs receivedEventArgs)
        {
            using (var dataReader = receivedEventArgs.GetDataReader())
            {
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                await ParseMessageAndSend(message);
            }
        }

        public async Task ParseMessageAndSend(string message)
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

        public void Close(string reason = "Application Closed")
        {
            if (messageWebSocket != null)
            {
                messageWebSocket.Close(1000, reason);
            }
        }
    }
}