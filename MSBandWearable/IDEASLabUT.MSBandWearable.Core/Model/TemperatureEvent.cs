using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    public class TemperatureEvent : BaseEvent
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
    }
}
