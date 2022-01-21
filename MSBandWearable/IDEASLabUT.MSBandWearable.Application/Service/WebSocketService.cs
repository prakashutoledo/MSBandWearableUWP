using System;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class WebSocketService
    {
        private static readonly Lazy<WebSocketService> Instance = new Lazy<WebSocketService>(() => new WebSocketService(new MessageWebSocket()));

        // Lazy singleton pattern
        public static WebSocketService Singleton => Instance.Value;

        public delegate Task MessageReceivedHandler(string message);
        private event MessageReceivedHandler OnMessageReceived;

        private readonly MessageWebSocket messageWebSocket;

        private WebSocketService(MessageWebSocket messageWebSocket)
        {
            this.messageWebSocket = messageWebSocket;
        }

        public async Task Connect(string webSocketUrl, MessageReceivedHandler onMessageReceived)
        {
            if(onMessageReceived != null)
            {
                OnMessageReceived += onMessageReceived;
            }

            await messageWebSocket.ConnectAsync(new Uri(webSocketUrl)).AsTask().ConfigureAwait(false);
            messageWebSocket.MessageReceived += MessageReceivedEvent;
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
                if (OnMessageReceived != null)
                {
                    await OnMessageReceived.Invoke(message).ConfigureAwait(false);
                }
                messageWebSocket.Dispose();
            }
        }

        public void Close(string reason = "Application Closed")
        {
            messageWebSocket.Close(1000, reason);
        }
    }
}