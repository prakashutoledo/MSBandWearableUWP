using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    /// <summary>
    /// POCO holding MS Band 2 rr interval or ibi sensor event details
    /// </summary>
    public class RRIntervalEvent : BaseEvent
    {
        /// <summary>
        /// An inter beats interval value
        /// </summary>
        [JsonProperty("ibi")]
        public double Ibi { get; set; }
    }
}
