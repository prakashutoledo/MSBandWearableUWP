using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model.Notification
{
    /// <summary>
    /// POCO for holding device serial number and connection status.
    /// Example used in this application is Empatica E4 device details
    /// </summary>
    public class Device
    {
        /// <summary>
        /// A unique serial number of the device
        /// </summary>
        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// A connection status of the device
        /// <code>true</code> represents connected
        /// <code>false</code> represents not connected
        /// </summary>
        [JsonProperty("connected")]
        public bool Connected { get; set; } = false;

        /// <summary>
        /// Returns a serialized json representation of <see cref="Device"/>
        /// </summary>
        /// <returns>A serialized json string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
