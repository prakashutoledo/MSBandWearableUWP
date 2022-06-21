using IDEASLabUT.MSBandWearable.Json;

using Newtonsoft.Json;

using static IDEASLabUT.MSBandWearable.Extension.JsonStringExtension;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// A webSocket notification message base POCO. Generally this base class can be abstract but
    /// it is not abstract as this is used in JSON deserialization to create an instance object
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        /// A message payload action
        /// </summary>
        [JsonConverter(typeof(PayloadActionConverter))]
        public PayloadAction? Action { get; set; }

        /// <summary>
        /// A message payload type
        /// </summary>
        [JsonConverter(typeof(PayloadTypeConverter))]
        public PayloadType? PayloadType { get; set; }

        /// <summary>
        /// Returns a serialized json representation of <see cref="BaseMessage"/>
        /// </summary>
        /// <returns>A serialized json string</returns>
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
