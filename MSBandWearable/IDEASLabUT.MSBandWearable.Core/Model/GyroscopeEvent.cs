using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    /// <summary>
    /// POCO holding MS Band 2 gyroscope sensor event details
    /// </summary>
    public class GyroscopeEvent : BaseEvent
    {
        /// <summary>
        /// An angular velocity value in X direction
        /// </summary>
        [JsonProperty("angularX")]
        public double AngularX { get; set; }

        /// <summary>
        /// An angular velocity value in Y direction
        /// </summary>
        [JsonProperty("angularY")]
        public double AngularY { get; set; }

        /// <summary>
        /// An angular velocity value in Z direction
        /// </summary>
        [JsonProperty("angularZ")]
        public double AngularZ { get; set; }
    }
}
