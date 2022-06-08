using System;
using System.Threading.Tasks;

using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    public class Utf8MessageWebSocket : IUtf8MessageWebSocket
    {
        public static Func<IUtf8MessageWebSocket> SocketSupplier = () => new Utf8MessageWebSocket();

        private readonly MessageWebSocket messageWebSocket;
        private readonly IDataWriter dataWriter;

        private Utf8MessageWebSocket()
        {
            messageWebSocket = new MessageWebSocket();
            dataWriter = new DataWriter(messageWebSocket.OutputStream);
        }

        public Func<string, Task> OnMessageReceived { get; set; }

        public IDataWriter DataWriter => dataWriter;

        public async Task ConnectAsync(string webSocketUrl)
        {
            messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
            messageWebSocket.MessageReceived += MessageReceivedEvent;
            await messageWebSocket.ConnectAsync(new Uri(webSocketUrl));
        }

        private async void MessageReceivedEvent(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs receivedEventArgs)
        {
            if (receivedEventArgs.MessageType != SocketMessageType.Utf8)
            {
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

        public void Close(ushort code, string reason)
        {
            messageWebSocket?.Close(code, reason);
        }

        public void Dispose()
        {
            dataWriter?.DetachStream();
            messageWebSocket?.Dispose();
        }
    }
}
