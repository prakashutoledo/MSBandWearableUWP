using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    public class GSREvent : BaseEvent
    {
        [JsonProperty("gsr")]
        public double Gsr { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
