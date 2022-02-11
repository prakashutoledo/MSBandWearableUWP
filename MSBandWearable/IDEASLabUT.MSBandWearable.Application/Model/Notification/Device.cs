using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Application.Model.Notification
{
    public class Device
    {
        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("connected")]
        public bool Connected { get; set; } = false;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
