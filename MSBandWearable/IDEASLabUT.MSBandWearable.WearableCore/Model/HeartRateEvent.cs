/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.Band.Sensors;

using System.Text.Json.Serialization;

namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// POCO holding MS Band 2 heart rate sensor event details
    /// </summary>
    public class HeartRateEvent : BaseEvent
    {
        /// <summary>
        /// A heart rate beats per minute value
        /// </summary>
        public int Bpm { get; set; }

        [JsonIgnore]
        public HeartRateQuality HeartRateStatus { get; set; }
    }
}
