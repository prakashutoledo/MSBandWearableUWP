using IDEASLabUT.MSBandWearable.Core.Json;
using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model.Notification
{
    public class BaseMessage
    {
        [JsonConverter(typeof(PayloadActionConverter))]
        [JsonProperty("action")]
        public PayloadAction Action { get; set; }

        [JsonConverter(typeof(PayloadTypeConverter))]
        [JsonProperty("payloadType")]
        public PayloadType PayloadType { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
