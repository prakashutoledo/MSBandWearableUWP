using IDEASLabUT.MSBandWearable.Application.Model.Notification;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
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

        public delegate Task MessageReceivedHandler(EmpaticaE4Band empaticaE4Band);
        private event MessageReceivedHandler OnEmpaticaE4BandMessageReceived;

        private MessageWebSocket messageWebSocket;

        private WebSocketService()
        {
        }

        public async Task Connect(string webSocketUrl, MessageReceivedHandler onEmpaticaE4BandMessageReceived)
        {
            messageWebSocket = new MessageWebSocket();
            if(onEmpaticaE4BandMessageReceived != null)
            {
                OnEmpaticaE4BandMessageReceived += onEmpaticaE4BandMessageReceived;
            }
            messageWebSocket.MessageReceived += MessageReceivedEvent;
            messageWebSocket.Control.MessageType = SocketMessageType.Utf8;

            await messageWebSocket.ConnectAsync(new Uri(webSocketUrl)).AsTask().ConfigureAwait(false);
        }


        public async Task SendMessage(string message)
        {
            using (var dataWriter = new DataWriter(messageWebSocket.OutputStream))
            {
                dataWriter.WriteString(message);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
        }

        public async void MessageReceivedEvent(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs receivedEventArgs)
        {
            using (var dataReader = receivedEventArgs.GetDataReader())
            {
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                await ParseMessageAndSend(message).ConfigureAwait(false);
            }
        }

        public async Task ParseMessageAndSend(string message)
        {
            if (message == null)
            {
                return;
            }

            var baseaMessage = JsonConvert.DeserializeObject<BaseMessage>(message);

            if (baseaMessage == null)
            {
                return;
            }

            switch(baseaMessage.PayloadType)
            {
                case PayloadType.E4Band:
                    var empaticaE4BandMessage = JsonConvert.DeserializeObject<EmpaticaE4BandMessage>(message);

                    if (OnEmpaticaE4BandMessageReceived != null)
                    {
                        await OnEmpaticaE4BandMessageReceived.Invoke(empaticaE4BandMessage.Payload).ConfigureAwait(false);
                    }
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