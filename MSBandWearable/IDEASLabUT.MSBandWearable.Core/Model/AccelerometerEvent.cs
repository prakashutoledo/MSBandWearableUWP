using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    /// <summary>
    /// POCO holding MS Band 2 accelerometer sensor event details
    /// </summary>
    public class AccelerometerEvent : BaseEvent
    {
        /// <summary>
        /// A linear acceleration value in X direction
        /// </summary>
        [JsonProperty("accelerationX")]
        public double AccelerationX { get; set; }

        /// <summary>
        /// A linear acceleration value in Y direction
        /// </summary>
        [JsonProperty("accelerationY")]
        public double AccelerationY { get; set; }

        /// <summary>
        /// A linear acceleration value in Z direction
        /// </summary>
        [JsonProperty("accelerationZ")]
        public double AccelerationZ { get; set; }
    }
}
