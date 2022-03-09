using IDEASLabUT.MSBandWearable.Core.Json;
using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model.Notification
{
    public class EmpaticaE4Band : Payload
    {
        [JsonConverter(typeof(PayloadTypeConverter))]
        [JsonProperty("payloadType")]
        public PayloadType PayloadType { get; } = PayloadType.E4Band;

        [JsonProperty("subjectId")]
        public string SubjectId { get; set; }

        [JsonProperty("fromView")]
        public string FromView { get; set; }

        [JsonProperty("device")]
        public Device Device { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
