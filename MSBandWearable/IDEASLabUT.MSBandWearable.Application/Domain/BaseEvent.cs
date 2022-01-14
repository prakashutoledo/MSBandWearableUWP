using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Domain
{
    public abstract class BaseEvent
    {
        public string AcquiredTime { get; set; }
        public string ActualTime { get; set; }
        public string BandType { get; private set; } = "MSBAND";
        public string FromView { get; set; }
        public string SubjectId { get; set; }
    }
}
