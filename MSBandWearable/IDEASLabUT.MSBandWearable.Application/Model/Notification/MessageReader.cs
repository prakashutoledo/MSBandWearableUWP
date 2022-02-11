using IDEASLabUT.MSBandWearable.Application.Json;
using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Application.Model.Notification
{
    public class MessageReader
    {
        [JsonConverter(typeof(PayloadTypeConverter))]
        [JsonProperty("payloadType")]
        public PayloadType PayloadType { get; set; }
    }
}
