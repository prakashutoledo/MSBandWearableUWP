using IDEASLabUT.MSBandWearable.Core.Json;
using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model.Notification
{
    /// <summary>
    /// An Empatica E4 Band device details POCO
    /// </summary>
    public class EmpaticaE4Band : IPayload
    {
        /// <summary>
        /// An E4Band payload type
        /// </summary>
        [JsonConverter(typeof(PayloadTypeConverter))]
        [JsonProperty("payloadType")]
        public PayloadType PayloadType { get; } = PayloadType.E4Band;

        /// <summary>
        /// A subject id of the subject wearing this E4 band
        /// </summary>
        [JsonProperty("subjectId")]
        public string SubjectId { get; set; }

        /// <summary>
        /// A current SwiftUI view of iPad used by subject
        /// </summary>
        [JsonProperty("fromView")]
        public string FromView { get; set; }

        /// <summary>
        /// And Empatica E4 band device details 
        /// </summary>
        [JsonProperty("device")]
        public Device Device { get; set; }

        /// <summary>
        /// Returns a serialized json representation of <see cref="BaseMessage"/>
        /// </summary>
        /// <returns>A serialized json string</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
