using HyperMock;
using IDEASLabUT.MSBandWearable.Core.Service;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Linq.Expressions;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    [TestClass]
    public class BaseSensorTest<T> where T : IBandSensorReading
    {
        protected Mock<ILogger> logger = null;
        protected Mock<IBandClientService> bandClientService = null;
        protected Mock<ISubjectViewService> subjectViewService = null;
        protected Mock<INtpSyncService> ntpSyncService = null;
        protected Mock<IBandClient> band = null;
        protected Mock<IBandSensorManager> sensorManager = null;
        protected Mock<T> sensorReading = null;
        protected Mock<IBandSensor<T>> sensor = null;

        [TestInitialize]
        public void Initialize()
        {
            logger = Mock.Create<ILogger>();
            bandClientService = Mock.Create<IBandClientService>();
            subjectViewService = Mock.Create<ISubjectViewService>();
            ntpSyncService = Mock.Create<INtpSyncService>();
            band = Mock.Create<IBandClient>();
            sensorManager = Mock.Create<IBandSensorManager>();
            sensorReading = Mock.Create<T>();
            sensor = Mock.Create<IBandSensor<T>>();

            band.SetupGet(band => band.SensorManager).Returns(sensorManager.Object);
            bandClientService.SetupGet(s => s.BandClient).Returns(band.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            logger = null;
            bandClientService = null;
            subjectViewService = null;
            ntpSyncService = null;
            band = null;
            sensorManager = null;
            sensorReading = null;
            sensor = null;
        }


        protected void MockSensorReadingChanged<R>(bool isNullReading, params (Expression<Func<T, R>> expression, R returns)[] whenAll)
        {
            foreach (var setup in whenAll)
            {
                sensorReading.SetupGet(setup.expression).Returns(setup.returns);
            }

            sensorReading.SetupGet(sensorReading => sensorReading.Timestamp).Returns(DateTime.Now);
            subjectViewService.SetupGet(subjectViewService => subjectViewService.CurrentView).Returns("Any View");
            subjectViewService.SetupGet(subjectViewService => subjectViewService.SubjectId).Returns("Fake Id");
            subjectViewService.SetupGet(subjectViewService => subjectViewService.SessionInProgress).Returns(true);
            ntpSyncService.SetupGet(ntpSyncService => ntpSyncService.CorrectionOffset).Returns(TimeSpan.Zero);
            sensor.Raise(sensor => sensor.ReadingChanged += null, new BandSensorReadingEventArgs<T>(isNullReading ? default : sensorReading.Object));
        }
        protected BandSensorReadingEventArgs<T> Null()
        {
            return new BandSensorReadingEventArgs<T>(default);
        }

        protected (Expression<Func<T, R>>, R) When<R>(Expression<Func<T, R>> expression, R returns) 
        {
            return (expression, returns);
        }
    }
}
