using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    /// <summary>
    /// POCO holding MS Band 2 heart rate sensor event details
    /// </summary>
    public class HeartRateEvent : BaseEvent
    {
        /// <summary>
        /// A heart rate beats per minute value
        /// </summary>
        [JsonProperty("bpm")]
        public double Bpm { get; set; }
    }
}
