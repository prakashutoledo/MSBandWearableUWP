using static HyperMock.Occurred;
using static Microsoft.Band.UserConsent;

using IDEASLabUT.MSBandWearable.Core.ViewModel;
using IDEASLabUT.MSBandWearable.Core.Model;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using System;
using HyperMock;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
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
            sensor.Setup(sensor => sensor.GetCurrentUserConsent()).Returns(Granted);
            sensor.Setup(sensor => sensor.StartReadingsAsync()).Returns(Task.FromResult(true));

            var status = await viewModel.Subscribe();

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

            var status = await viewModel.Subscribe();

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

            var status = await viewModel.Subscribe();

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

            var status = await viewModel.Subscribe();

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

            var status = await viewModel.Subscribe();

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

            await MockSensorReadingChanged(When(r => r.AccelerationX, 1.0), When(r => r.AccelerationY, 2.0), When(r => r.AccelerationZ, 3.0));

            var actualModel = viewModel.Model;
            var expectedModel = NewModel(value =>
            {
                value.AccelerationX = 1.0;
                value.AccelerationY = 2.0;
                value.AccelerationZ = 3.0;
            });

            Assert.AreEqual(expectedModel.ToString(), actualModel.ToString());
            subjectViewService.VerifyGet(subjectViewService => subjectViewService.CurrentView, Once());
            subjectViewService.VerifyGet(subjectViewService => subjectViewService.SubjectId, Once());
            ntpSyncService.VerifyGet(ntpSyncService => ntpSyncService.LocalTimeNow, Once());
            subjectViewService.VerifyGet(subjectViewService => subjectViewService.SessionInProgress, Once());
            logger.Verify(logger => logger.Information("{accelerometer}", Param.IsAny<AccelerometerEvent>()), Once());
        }
    }
}
