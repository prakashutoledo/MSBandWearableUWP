using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Model
{
    public class AccelerometerEvent : BaseEvent
    {
        [JsonProperty("accelerationX")]
        public double AccelerationX { get; set; }

        [JsonProperty("accelerationY")]
        public double AccelerationY { get; set; }

        [JsonProperty("accelerationZ")]
        public double AccelerationZ { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
