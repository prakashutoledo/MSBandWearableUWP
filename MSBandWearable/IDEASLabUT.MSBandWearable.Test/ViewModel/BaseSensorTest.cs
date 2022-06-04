using HyperMock;
using IDEASLabUT.MSBandWearable.Core.Service;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;
using System.Threading;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    [TestClass]
    public class BaseSensorTest<T, R> where T : BaseEvent, new() where R : IBandSensorReading
    {
        protected Mock<ILogger> logger = null;
        protected Mock<IBandClientService> bandClientService = null;
        protected Mock<ISubjectViewService> subjectViewService = null;
        protected Mock<INtpSyncService> ntpSyncService = null;
        protected Mock<IBandClient> band = null;
        protected Mock<IBandSensorManager> sensorManager = null;
        protected Mock<R> sensorReading = null;
        protected Mock<IBandSensor<R>> sensor = null;
        protected BaseSensorViewModel<T, R> viewModel = null;
        private Expression<Func<IBandSensorManager, IBandSensor<R>>> sensorExpression;

        protected BaseSensorTest(Expression<Func<IBandSensorManager, IBandSensor<R>>> sensorExpression)
        {
            this.sensorExpression = sensorExpression;
        }

        [TestInitialize]
        public void Initialize()
        {
            logger = Mock.Create<ILogger>();
            bandClientService = Mock.Create<IBandClientService>();
            subjectViewService = Mock.Create<ISubjectViewService>();
            ntpSyncService = Mock.Create<INtpSyncService>();
            band = Mock.Create<IBandClient>();
            sensorManager = Mock.Create<IBandSensorManager>();
            sensorReading = Mock.Create<R>();
            sensor = Mock.Create<IBandSensor<R>>();

            band.SetupGet(band => band.SensorManager).Returns(sensorManager.Object);
            bandClientService.SetupGet(bandClientService => bandClientService.BandClient).Returns(band.Object);
            sensorManager.SetupGet(sensorExpression).Returns(sensor.Object);
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

        protected async Task MockSensorReadingChanged<E>(params (Expression<Func<R, E>> when, E then)[] whenAll)
        {
            var awaitLatch = new AutoResetEvent(false);
            viewModel.SensorModelChanged = async _ =>
            {
                awaitLatch.Set();
                await Task.CompletedTask;
            };
            _ = await viewModel.Subscribe();

            foreach (var setup in whenAll)
            {
                sensorReading.SetupGet(setup.when).Returns(setup.then);
            }

            sensorReading.SetupGet(sensorReading => sensorReading.Timestamp).Returns(Param.IsAny<DateTime>());
            subjectViewService.SetupGet(subjectViewService => subjectViewService.CurrentView).Returns("Fake View");
            subjectViewService.SetupGet(subjectViewService => subjectViewService.SubjectId).Returns("Fake Id");
            subjectViewService.SetupGet(subjectViewService => subjectViewService.SessionInProgress).Returns(true);
            ntpSyncService.SetupGet(ntpSyncService => ntpSyncService.LocalTimeNow).Returns(Param.IsAny<DateTime>());
            sensor.Raise(sensor => sensor.ReadingChanged += null, new BandSensorReadingEventArgs<R>(sensorReading.Object));
            awaitLatch.WaitOne();
        }

        protected (Expression<Func<R, E>> when, E then) When<E>(Expression<Func<R, E>> when, E then)
        {
            return (when, then);
        }

        protected T NewModel(Action<T> update)
        {
            var modelEvent = new T();
            update.Invoke(modelEvent);
            modelEvent.AcquiredTime = Param.IsAny<DateTime>();
            modelEvent.ActualTime = Param.IsAny<DateTime>();
            modelEvent.FromView = "Fake View";
            modelEvent.SubjectId = "Fake Id";
            return modelEvent;
        }
    }
}
