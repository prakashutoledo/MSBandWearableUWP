/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model.Notification;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A utf8 message web socket service
    /// </summary>
    public interface IWebSocketService
    {
        /// <summary>
        /// Gets all the message post processor for converting raw websocket message to process
        /// </summary>
        IReadOnlyDictionary<PayloadType, Func<object, Task>> GetMessagePostProcessors { get; }

        /// <summary>
        /// Connect to given webSocket url by setting given webSocket message received callback function
        /// </summary>
        /// <param name="webSocketUrl">A webSocket URL to connect</param>
        /// <returns>A task that can be awaited</returns>
        Task Connect(string webSocketUrl, Func<bool, Task> continueWith = null);

        /// <summary>
        /// Sends the given message as webSocket message
        /// </summary>
        /// <typeparam name="Payload">A type of payload holding by given message</typeparam>
        /// <param name="message">A webSocket mesage to send</param>
        /// <param name="continueWith">An asynchronous callback function to continue with after sending message</param>
        /// <returns>A task that can be awaited</returns>
        Task SendMessage<Payload>(Message<Payload> message, Func<bool, Task> continueWith = null) where Payload : IPayload;

        /// Adds the message post processor from given payload type. If such processor exists for
        /// given type, it will replace old processor with new processor value
        /// </summary>
        /// <param name="type">A type of payload to set message post processor</param>
        /// <param name="processor">A message post processor to set</param>
        void AddMessagePostProcessor<Payload>(PayloadType payloadType, Func<Payload, Task> processor) where Payload : IPayload;

        /// <summary>
        /// Close the given message WebSocket instance with given reason
        /// </summary>
        /// <param name="reason">A reason phrase to close</param>
        void Close(string reason = "Application Closed");
    }
}
