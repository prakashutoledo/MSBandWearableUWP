using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    /// <summary>
    /// POCO holding MS Band 2 GSR sensor event details
    /// </summary>
    public class GSREvent : BaseEvent
    {
        /// <summary>
        /// A current gsr resistance value in KOHMs
        /// </summary>
        [JsonProperty("gsr")]
        public double Gsr { get; set; }
    }
}
