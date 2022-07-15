/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model.Notification;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Networking.Sockets;

using static IDEASLabUT.MSBandWearable.Extension.TaskExtension;
using static IDEASLabUT.MSBandWearable.Model.Notification.Utf8MessageWebSocket;
using static IDEASLabUT.MSBandWearable.Util.WebSocketUtil;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A service class for managing webSocket connection details using <see cref="MessageWebSocket"/>
    /// </summary>
    public sealed class WebSocketService : IWebSocketService
    {
        private static readonly Lazy<IWebSocketService> Instance = new Lazy<IWebSocketService>(() => new WebSocketService());

        // Lazy singleton pattern
        internal static IWebSocketService Singleton => Instance.Value;

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

            messageWebSocket = SocketSupplier.Invoke();
            async Task OnMessageReceived(string message)
            {
                _ = await ParseMessageAndProcess(message, GetMessagePostProcessors);
            }
            messageWebSocket.OnMessageReceived = OnMessageReceived;
            await messageWebSocket.ConnectAsync(webSocketUrl).ContinueWithStatusSupplier(continueWith);
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
            var dataWriter = messageWebSocket.MessageWriter;
            _ = await dataWriter.FlushAsync();
            _ = dataWriter.WriteString(message.ToString());
            await dataWriter.StoreAsync().AsTask().ContinueWithStatusSupplier(continueWith);
        }

        /// <summary>
        /// Adds the message post processor from given payload type. If such processor exists for
        /// given type, it will replace old processor with new processor value
        /// </summary>
        /// <param name="type">A type of payload to set message post processor</param>
        /// <param name="processor">A message post processor to set</param>
        public void AddMessagePostProcessor<Payload>(PayloadType type, Func<Payload, Task> processor) where Payload : IPayload
        {
            if (processor == null)
            {
                return;
            }

            Task GenericPayloadProcessor(object message)
            {
                return processor.Invoke((message as Message<Payload>).Payload);
            }

            // Add new processor or replace existing existing processor
            if (!processors.TryAdd(type, GenericPayloadProcessor))
            {
                processors[type] = GenericPayloadProcessor;
            }
        }

        /// <summary>
        /// Close the given message WebSocket instance with given reason
        /// </summary>
        /// <param name="reason">A reason phrase to close</param>
        public void Close(string reason = "Application Closed")
        {
            messageWebSocket?.Close(1000, reason);
            messageWebSocket = null;
        }
    }
}
