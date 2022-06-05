using static HyperMock.Occurred;
using static Microsoft.Band.UserConsent;

using IDEASLabUT.MSBandWearable.Core.ViewModel;
using IDEASLabUT.MSBandWearable.Core.Model;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    /// <summary>
    /// Accelerometer sensor view model test
    /// </summary>
    [TestClass]
    public class AccelerometerSensorTest : BaseSensorTest<AccelerometerEvent, IBandAccelerometerReading>
    {
        public AccelerometerSensorTest() : base(sensorManager => sensorManager.Accelerometer)
        {
        }

        [TestInitialize]
        public void InitializeAccelerometer()
        {
            viewModel = new AccelerometerSensor(logger.Object, bandClientService.Object, subjectViewService.Object, ntpSyncService.Object);
        }
        
        [TestMethod]
        public async Task SubscribeWithCurrentUserConsentSuccess()
        {
            var status = await MockSubscribe();
            VerifySubscribe(expectedStatus: true, actualStatus: status, bandClientOccurred: Once(), sensorManagerOccurred: Once(), getConsentOccurred: Once(), requestConsentOccurred: Never(), startReadingOccurred: Once());
        }


        [TestMethod]
        public async Task SubscribeWithRequestUserConsentSuccess()
        {
            var status = await MockSubscribe(currentUserConsent: NotSpecified, requestUserAsync: true);
            VerifySubscribe(expectedStatus: true, actualStatus: status, bandClientOccurred: Once(), sensorManagerOccurred: Once(), getConsentOccurred: Once(), requestConsentOccurred: Once(), startReadingOccurred: Once());
        }

        [TestMethod]
        public async Task SubscribeWithRequestUserConsentReadFailure()
        {
            var status = await MockSubscribe(currentUserConsent: Granted, startReadingAsync: false);
            VerifySubscribe(expectedStatus: false, actualStatus: status, bandClientOccurred: Once(), sensorManagerOccurred: Once(), getConsentOccurred: Once(), requestConsentOccurred: Never(), startReadingOccurred: Once());
        }

        [TestMethod]
        public async Task SubscribeWithRequestUserConsentNotGranted()
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

            VerifySensorValueChanged(expectedModel: expectedModel);
        }
    }
}
