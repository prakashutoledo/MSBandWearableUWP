using static HyperMock.Occurred;
using static Microsoft.Band.UserConsent;

using IDEASLabUT.MSBandWearable.Core.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using IDEASLabUT.MSBandWearable.Core.Model;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    [TestClass]
    public class AccelerometerSensorTest : BaseSensorTest<IBandAccelerometerReading>
    {
        private AccelerometerSensor accelerometer = null;

        [TestInitialize]
        public void InitializeAccelerometer()
        {
            sensorManager.SetupGet(sensorManager => sensorManager.Accelerometer).Returns(sensor.Object);
            accelerometer = new AccelerometerSensor(logger.Object, bandClientService.Object, subjectViewService.Object, ntpSyncService.Object);
        }
        
        [TestMethod]
        public async Task SubscribeWithCurrentUserConsentSuccess()
        {
            sensor.Setup(sensor => sensor.GetCurrentUserConsent()).Returns(Granted);
            sensor.Setup(sensor => sensor.StartReadingsAsync()).Returns(Task.FromResult(true));

            var status = await accelerometer.Subscribe();

            Assert.IsTrue(status);
            bandClientService.VerifyGet(bandClientService => bandClientService.BandClient, Once());
            sensorManager.VerifyGet(sensorManager => sensorManager.Accelerometer, Once());
            sensor.Verify(sensor => sensor.GetCurrentUserConsent(), Once());
            sensor.Verify(sensor => sensor.RequestUserConsentAsync(), Never());
            sensor.Verify(sensor => sensor.StartReadingsAsync(), Once());
        }


        [TestMethod]
        public async Task SubscribeWithRequestUserConsentSuccess()
        {
            sensor.Setup(sensor => sensor.RequestUserConsentAsync()).Returns(Task.FromResult(true));
            sensor.Setup(sensor => sensor.StartReadingsAsync()).Returns(Task.FromResult(true));

            var status = await accelerometer.Subscribe();

            Assert.IsTrue(status);
            bandClientService.VerifyGet(bandClientService => bandClientService.BandClient, Once());
            sensorManager.VerifyGet(sensorManager => sensorManager.Accelerometer, Once());
            sensor.Verify(sensor => sensor.GetCurrentUserConsent(), Once());
            sensor.Verify(sensor => sensor.RequestUserConsentAsync(), Once());
            sensor.Verify(sensor => sensor.StartReadingsAsync(), Once());
        }

        [TestMethod]
        public async Task SubscribeWithRequestUserConsentFailure()
        {
            sensor.Setup(sensor => sensor.RequestUserConsentAsync()).Returns(Task.FromResult(false));

            var status = await accelerometer.Subscribe();

            Assert.IsFalse(status);
            bandClientService.VerifyGet(bandClientService => bandClientService.BandClient, Once());
            sensorManager.VerifyGet(sensorManager => sensorManager.Accelerometer, Once());
            sensor.Verify(sensor => sensor.GetCurrentUserConsent(), Once());
            sensor.Verify(sensor => sensor.RequestUserConsentAsync(), Once());
            sensor.Verify(sensor => sensor.StartReadingsAsync(), Never());
        }

        [TestMethod]
        public async Task SubscribeWithRequestUserConsentReadFailure()
        {
            sensor.Setup(sensor => sensor.GetCurrentUserConsent()).Returns(Granted);
            sensor.Setup(sensor => sensor.StartReadingsAsync()).Returns(Task.FromResult(false));

            var status = await accelerometer.Subscribe();

            Assert.IsFalse(status);
            bandClientService.VerifyGet(bandClientService => bandClientService.BandClient, Once());
            sensorManager.VerifyGet(sensorManager => sensorManager.Accelerometer, Once());
            sensor.Verify(sensor => sensor.GetCurrentUserConsent(), Once());
            sensor.Verify(sensor => sensor.RequestUserConsentAsync(), Never());
            sensor.Verify(sensor => sensor.StartReadingsAsync(), Once());
        }

        [TestMethod]
        public async Task SubscribeWithRequestUserConsentNotGranted()
        {
            sensor.Setup(sensor => sensor.GetCurrentUserConsent()).Returns(Declined);
            sensor.Setup(sensor => sensor.RequestUserConsentAsync()).Returns(Task.FromResult(false));

            var status = await accelerometer.Subscribe();

            Assert.IsFalse(status);
            bandClientService.VerifyGet(bandClientService => bandClientService.BandClient, Once());
            sensorManager.VerifyGet(sensorManager => sensorManager.Accelerometer, Once());
            sensor.Verify(sensor => sensor.GetCurrentUserConsent(), Once());
            sensor.Verify(sensor => sensor.RequestUserConsentAsync(), Once());
            sensor.Verify(sensor => sensor.StartReadingsAsync(), Never());
        }

        [TestMethod]
        public async Task OnAccelerometerReadingChanged()
        {
            sensor.Setup(sensor => sensor.GetCurrentUserConsent()).Returns(Granted);
            sensor.Setup(sensor => sensor.StartReadingsAsync()).Returns(Task.FromResult(true));

            _ = await accelerometer.Subscribe();
            MockSensorReadingChanged(false, When(reading => reading.AccelerationX, 1.0), When(reading => reading.AccelerationY, 2.0), When(reading => reading.AccelerationZ, 3.0));

            
            var model = accelerometer.Model;

            Assert.IsNotNull(model);
            Assert.AreEqual(1.0, model.AccelerationX);
            Assert.AreEqual(2.0, model.AccelerationY);
            Assert.AreEqual(3.0, model.AccelerationZ);
            Assert.IsNotNull(model.AcquiredTime);
            Assert.IsNotNull(model.ActualTime);
            Assert.AreEqual(BandType.MSBand, model.BandType);
            Assert.AreEqual("Any View", model.FromView);
            Assert.AreEqual("Fake Id", model.SubjectId);
        }
    }
}
