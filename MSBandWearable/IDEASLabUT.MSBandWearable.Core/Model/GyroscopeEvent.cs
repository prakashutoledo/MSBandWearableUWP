﻿using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    public class GyroscopeEvent : BaseEvent
    {
        [JsonProperty("angularX")]
        public double AngularX { get; set; }

        [JsonProperty("angularY")]
        public double AngularY { get; set; }

        [JsonProperty("angularZ")]
        public double AngularZ { get; set; }
    }
}
