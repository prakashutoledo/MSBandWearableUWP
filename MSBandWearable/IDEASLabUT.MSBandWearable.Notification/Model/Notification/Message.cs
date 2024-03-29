﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// An webSocket message details POCO
    /// </summary>
    /// <typeparam name="T">A parameter of type <see cref="IPayload"/></typeparam>
    public class Message<T> : BaseMessage where T : IPayload
    {
        /// <summary>
        /// A message payload hold by this webSocket message
        /// </summary>
        public T Payload { get; set; }
    }
}
