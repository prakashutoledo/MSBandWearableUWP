﻿using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    /// <summary>
    /// POCO holding MS Band 2 temperature sensor event details
    /// </summary>
    public class TemperatureEvent : BaseEvent
    {
        /// <summary>
        /// A temperature value in degree celsius (°C)
        /// </summary>
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
    }
}
