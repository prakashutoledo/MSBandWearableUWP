using IDEASLabUT.MSBandWearable.Application.Json;
using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Application.Model
{
    public abstract class BaseEvent
    {
        [JsonConverter(typeof(ZonedDateTimeOptionalNanoConverter))]
        [JsonProperty("acquiredTime")]
        public DateTime AcquiredTime { get; set; }

        [JsonConverter(typeof(ZonedDateTimeOptionalNanoConverter))]
        [JsonProperty("actualTime")]
        public DateTime ActualTime { get; set; }

        [JsonProperty("bandType")]
        public string BandType { get; private set; } = MSBandWearableApplicationGlobals.MSBandTypeName;

        [JsonProperty("fromView")]
        public string FromView { get; set; }

        [JsonProperty("subjectId")]
        public string SubjectId { get; set; }
    }
}
