using IDEASLabUT.MSBandWearable.Core.Json;
using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    public abstract class BaseEvent
    {
        [JsonConverter(typeof(ZonedDateTimeOptionalNanoConverter))]
        [JsonProperty("acquiredTime")]
        public DateTime AcquiredTime { get; set; }

        [JsonConverter(typeof(ZonedDateTimeOptionalNanoConverter))]
        [JsonProperty("actualTime")]
        public DateTime ActualTime { get; set; }

        [JsonConverter(typeof(BandTypeConverter))]
        [JsonProperty("bandType")]
        public BandType BandType { get; private set; } = BandType.MSBand;

        [JsonProperty("fromView")]
        public string FromView { get; set; }

        [JsonProperty("subjectId")]
        public string SubjectId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
