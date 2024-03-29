﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Json;

using System.Text.Json.Serialization;

using System;

using static IDEASLabUT.MSBandWearable.Extension.JsonStringExtension;

namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// An abstract class providing basis for all MS Band 2 sensor events common details
    /// </summary>
    public abstract class BaseEvent
    {
        /// <summary>
        /// A time which this event is acquired or accumulated or received
        /// </summary>
        [JsonConverter(typeof(ZonedDateTimeOptionalNanoConverter))]
        public DateTime AcquiredTime { get; set; }

        /// <summary>
        /// A time which this event is generated by sensor
        /// </summary>
        [JsonConverter(typeof(ZonedDateTimeOptionalNanoConverter))]
        public DateTime ActualTime { get; set; }

        /// <summary>
        /// A type of wearable band which this events belongs to
        /// </summary>
        [JsonConverter(typeof(BandTypeConverter))]
        public BandType BandType { get; private set; } = BandType.MSBand;

        /// <summary>
        /// An iOS SwiftUI view which subject is using in iPad wearing the band from which event is generated 
        /// </summary>
        public string FromView { get; set; }

        /// <summary>
        /// A unique identifier of subject in the running experiment wearing the band which generates this event
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// Returns a serialized json representation of <see cref="BaseEvent"/>
        /// </summary>
        /// <returns>A serialized json string</returns>
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
