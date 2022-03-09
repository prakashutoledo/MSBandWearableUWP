using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Core.Model.Notification
{
    public class Message<T> : BaseMessage where T : Payload
    {
        [JsonProperty("payload")]
        public T Payload { get; set; }
    }
}
