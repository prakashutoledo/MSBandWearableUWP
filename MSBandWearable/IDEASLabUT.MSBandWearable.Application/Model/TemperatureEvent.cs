using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Application.Model
{
    public class TemperatureEvent : BaseEvent
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
