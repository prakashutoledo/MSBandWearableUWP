using IDEASLabUT.MSBandWearable.Model.Notification;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.Networking.Sockets;

using static IDEASLabUT.MSBandWearable.Util.WebSocketUtil;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A service class for managing webSocket connection details using <see cref="MessageWebSocket"/>
    /// </summary>
    public class WebSocketService
    {
        private static readonly Lazy<WebSocketService> Instance = new Lazy<WebSocketService>(() => new WebSocketService());

        // Lazy singleton pattern
        public static WebSocketService Singleton => Instance.Value;

        private IUtf8MessageWebSocket messageWebSocket;
        private readonly Dictionary<PayloadType, Func<object, Task>> processors;

        /// <summary>
        /// Initializes a new instance of <see cref="WebSocketService"/>
        /// </summary>
        private WebSocketService()
        {
            processors = new Dictionary<PayloadType, Func<object, Task>>();
        }

        /// <summary>
        /// Gets all the message post processor for converting raw websocket message to process
        /// </summary>
        public IReadOnlyDictionary<PayloadType, Func<object, Task>> GetMessagePostProcessors => processors;

        /// <summary>
        /// Connect to given webSocket url by setting given webSocket message received callback function
        /// </summary>
        /// <param name="webSocketUrl">A webSocket URL to connect</param>
        /// <returns>A task that can be awaited</returns>
        public async Task Connect(string webSocketUrl, Func<bool, Task> continueWith = null)
        {
            messageWebSocket = Utf8MessageWebSocket.SocketSupplier.Invoke();
            messageWebSocket.OnMessageReceived = message => ParseMessageAndProcess(message, GetMessagePostProcessors);
            await messageWebSocket.ConnectAsync(webSocketUrl).ContinueWith(connect => continueWith?.Invoke(connect.IsCompleted && connect.Exception == null)).Unwrap();
        }

        /// <summary>
        /// Sends the given message as webSocket message
        /// </summary>
        /// <typeparam name="Payload">A type of payload holding by given message</typeparam>
        /// <param name="message">A webSocket mesage to send</param>
        /// <param name="continueWith">An asynchronous callback function to continue with after sending message</param>
        /// <returns>A task that can be awaited</returns>
        public async Task SendMessage<Payload>(Message<Payload> message, Func<bool, Task> continueWith = null) where Payload : IPayload
        {
            var dataWriter = messageWebSocket.DataWriter;
            _ = await dataWriter.FlushAsync();
            _ = dataWriter.WriteString(message.ToString());
            await dataWriter.StoreAsync().AsTask().ContinueWith(task => continueWith?.Invoke(task.IsCompleted && task.Exception == null)).Unwrap();
        }

        /// <summary>
        /// Sets the message post processor from given payload type
        /// </summary>
        /// <param name="type">A type of payload to set message post processor</param>
        /// <param name="processor">A message post processor to set</param>
        public void SetMessagePostProcessor(PayloadType type, Func<object, Task> processor)
        {
            if (processor == null)
            {
                return;
            }
            processors.Add(type, processor);
        }

        /// <summary>
        /// Serialize given webSocket raw json message and call the message received function
        /// </summary>
        /// <param name="message">A webSocket json message to be serialized</param>
        /// <returns>A task that can be awaited</returns>
        private async Task ParseMessageAndProcess1(string message)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Close the given message WebSocket instance with given reason
        /// </summary>
        /// <param name="reason"></param>
        public void Close(string reason = "Application Closed")
        {
            messageWebSocket?.Close(1000, reason);
            messageWebSocket = null;
        }
    }
}
