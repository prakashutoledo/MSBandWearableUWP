﻿using IDEASLabUT.MSBandWearable.Model.Notification;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Service
{
    public interface IWebSocketService
    {
        IReadOnlyDictionary<PayloadType, Func<object, Task>> GetMessagePostProcessors { get; }
        Task Connect(string webSocketUrl, Func<bool, Task> continueWith = null);
        Task SendMessage<Payload>(Message<Payload> message, Func<bool, Task> continueWith = null) where Payload : IPayload;
        void AddMessagePostProcessor<Payload>(PayloadType payloadType, Func<Payload, Task> processor) where Payload : IPayload;
        void Close(string reason = "Application Closed");
    }
}