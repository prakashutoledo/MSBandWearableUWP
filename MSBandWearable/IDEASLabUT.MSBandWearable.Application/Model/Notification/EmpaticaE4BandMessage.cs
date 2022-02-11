using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Application.Model.Notification
{
    public class EmpaticaE4BandMessage : Message<EmpaticaE4Band>
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
