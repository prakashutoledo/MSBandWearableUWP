using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    public class RRIntervalEvent : BaseEvent
    {
        [JsonProperty("ibi")]
        public double Ibi { get; set; }
    }
}
