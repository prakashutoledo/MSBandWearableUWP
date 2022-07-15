/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;
using System.Threading.Tasks;

using Windows.Storage.Streams;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// A disposable utf-8 message webSocket interface
    /// </summary>
    public interface IUtf8MessageWebSocket : IDisposable
    {
        /// <summary>
        /// Connects to given webSocketUrl asynchronously
        /// </summary>
        /// <param name="webSocketUrl">A webSocket url to connect</param>
        /// <returns>A task that can be awaited</returns>
        Task ConnectAsync(string webSocketUrl);

        /// <summary>
        /// A message received callback which will be called once websocket message is received
        /// </summary>
        Func<string, Task> OnMessageReceived { get; set; }

        /// <summary>
        /// A message received callback which will be called once websocket message is received
        /// </summary>
        IDataWriter MessageWriter { get; }

        /// <summary>
        /// Close the underlying webSocket instance with given code and reason
        /// </summary>
        /// <param name="code">A code to set for closing webSocket</param>
        /// <param name="reason">A reason phrase for closing</param>
        void Close(ushort code, string reason);

    }
}