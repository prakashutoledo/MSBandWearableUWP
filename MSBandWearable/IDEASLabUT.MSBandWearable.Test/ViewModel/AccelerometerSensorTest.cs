using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

using static HyperMock.Occurred;
using static Microsoft.Band.UserConsent;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    /// <summary>
    /// View model test for <see cref="AccelerometerSensor"/>
    /// </summary>
    [TestClass]
    public class AccelerometerSensorTest : BaseSensorTest<AccelerometerEvent, IBandAccelerometerReading>
    {
        public AccelerometerSensorTest() : base(sensorManager => sensorManager.Accelerometer, (logger, bandClientService, subjectViewService, ntpSyncService) => new AccelerometerSensor(logger, bandClientService, subjectViewService, ntpSyncService))
        {
        }
        
        [TestMethod]
        public async Task SubscribeAccelerometerSensorWithCurrentUserConsentSuccess()
        {
            var status = await MockSubscribe();
            VerifySubscribe(expectedStatus: true, actualStatus: status, bandClientOccurred: Once(), sensorManagerOccurred: Once(), getConsentOccurred: Once(), requestConsentOccurred: Never(), startReadingOccurred: Once());
        }


        [TestMethod]
        public async Task SubscribeAccelerometerSensorWithRequestUserConsentSuccess()
        {
            var status = await MockSubscribe(currentUserConsent: NotSpecified, requestUserAsync: true);
            VerifySubscribe(expectedStatus: true, actualStatus: status, bandClientOccurred: Once(), sensorManagerOccurred: Once(), getConsentOccurred: Once(), requestConsentOccurred: Once(), startReadingOccurred: Once());
        }

        [TestMethod]
        public async Task SubscribeAccelerometerSensorWithRequestUserConsentReadFailure()
        {
            var status = await MockSubscribe(currentUserConsent: Granted, startReadingAsync: false);
            VerifySubscribe(expectedStatus: false, actualStatus: status, bandClientOccurred: Once(), sensorManagerOccurred: Once(), getConsentOccurred: Once(), requestConsentOccurred: Never(), startReadingOccurred: Once());
        }

        [TestMethod]
        public async Task SubscribeAccelerometerSensorWithRequestUserConsentNotGranted()
        {
            var status = await MockSubscribe(currentUserConsent: Declined);
            VerifySubscribe(expectedStatus: false, actualStatus: status, bandClientOccurred: Once(), sensorManagerOccurred: Once(), getConsentOccurred: Once(), requestConsentOccurred: Once(), startReadingOccurred: Never());
        }

        [TestMethod]
        public async Task OnAccelerometerReadingChanged()
        {
            await MockSensorReadingChanged(When(r => r.AccelerationX, 1.0), When(r => r.AccelerationY, 2.0), When(r => r.AccelerationZ, 3.0));

            var expectedModel = NewModel(value =>
            {
                value.AccelerationX = 1.0;
                value.AccelerationY = 2.0;
                value.AccelerationZ = 3.0;
            });

            VerifySensorValueChanged(expectedModel);
        }
    }
}
