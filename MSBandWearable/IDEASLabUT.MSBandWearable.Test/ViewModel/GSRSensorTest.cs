using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    /// <summary>
    /// View model test for <see cref="GSRSensor"/>
    /// </summary>
    [TestClass]
    public class GSRSensorTest : BaseSensorTest<GSREvent, IBandGsrReading>
    {
        public GSRSensorTest() : base(sensorManager => sensorManager.Gsr, (logger, bandClientService, subjectViewService, ntpSyncService) => new GSRSensor(logger, bandClientService, subjectViewService, ntpSyncService))
        {
        }
    }
}
