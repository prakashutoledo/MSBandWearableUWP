using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    public class AccelerometerEvent : BaseEvent
    {
        [JsonProperty("accelerationX")]
        public double AccelerationX { get; set; }

        [JsonProperty("accelerationY")]
        public double AccelerationY { get; set; }

        [JsonProperty("accelerationZ")]
        public double AccelerationZ { get; set; }
    }
}
