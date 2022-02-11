using IDEASLabUT.MSBandWearable.Application.Json;
using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Application.Model.Notification
{
    public class BaseMessage
    {
        [JsonConverter(typeof(PayloadActionConverter))]
        [JsonProperty("action")]
        public PayloadAction Action { get; set; }

        [JsonConverter(typeof(PayloadTypeConverter))]
        [JsonProperty("payloadType")]
        public PayloadType PayloadType { get; set; }
    }
}
