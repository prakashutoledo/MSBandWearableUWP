﻿using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    public class HeartRateEvent : BaseEvent
    {
        [JsonProperty("bpm")]
        public double Bpm { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}