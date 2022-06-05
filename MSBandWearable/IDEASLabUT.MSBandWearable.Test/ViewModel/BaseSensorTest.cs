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
    /// <summary>
    /// Base sensor model test for all view sensor models
    /// </summary>
    /// <typeparam name="T">A parameter of type <see cref="BaseEvent"/></typeparam>
    /// <typeparam name="R">A parameter of type <see cref="IBandSensorReading"/></typeparam>
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
        private readonly Expression<Func<IBandSensorManager, IBandSensor<R>>> sensorExpression;

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

        /// <summary>
        /// Mocks sensor reading changed handler by seting all setup for mocking model value changed
        /// </summary>
        /// <typeparam name="E">A type of mock return value for when expression</typeparam>
        /// <param name="setups">All when then expression setup for mocking model change</param>
        /// <returns></returns>
        protected async Task MockSensorReadingChanged<E>(params (Expression<Func<R, E>> when, E then)[] setups)
        {
            var awaitLatch = new AutoResetEvent(false);
            viewModel.SensorModelChanged = async _ =>
            {
                awaitLatch.Set();
                await Task.CompletedTask;
            };

            _ = await MockSubscribe();

            foreach (var setup in setups)
            {
                sensorReading.SetupGet(setup.when).Returns(setup.then);
            }

            sensorReading.SetupGet(sensorReading => sensorReading.Timestamp).Returns(Param.IsAny<DateTime>());
            subjectViewService.SetupGet(subjectViewService => subjectViewService.CurrentView).Returns("Fake View");
            subjectViewService.SetupGet(subjectViewService => subjectViewService.SubjectId).Returns("Fake Id");
            subjectViewService.SetupGet(subjectViewService => subjectViewService.SessionInProgress).Returns(true);
            ntpSyncService.SetupGet(ntpSyncService => ntpSyncService.LocalTimeNow).Returns(Param.IsAny<DateTime>());

            // Raise Sensor reading change event
            sensor.Raise(sensor => sensor.ReadingChanged += null, new BandSensorReadingEventArgs<R>(sensorReading.Object));
            // Wait for signal to be received
            awaitLatch.WaitOne();
        }

        /// <summary>
        /// Mocks all properties for sensor subscription task
        /// </summary>
        /// <param name="currentUserConsent">A current user consent to set</param>
        /// <param name="requestUserAsync">A return value for request user async expression mock</param>
        /// <param name="startReadingAsync">A return for start reading async expression mock</param>
        /// <returns>An awaiatable task with sensor subscription status</returns>
        protected async Task<bool> MockSubscribe(UserConsent currentUserConsent = UserConsent.Granted, bool requestUserAsync = false, bool startReadingAsync = true)
        {
            sensor.Setup(sensor => sensor.GetCurrentUserConsent()).Returns(currentUserConsent);
            sensor.Setup(sensor => sensor.RequestUserConsentAsync()).Returns(Task.FromResult(requestUserAsync));
            sensor.Setup(sensor => sensor.StartReadingsAsync()).Returns(Task.FromResult(startReadingAsync));
            var status = await viewModel.Subscribe();
            return status;
        }

        /// <summary>
        /// Creates a tuple matching when mocked expression then returns mock value
        /// </summary>
        /// <typeparam name="E">A type of then parameter</typeparam>
        /// <param name="when">A when expression to setup for mock</param>
        /// <param name="then">A then return value for mocking expression</param>
        /// <returns></returns>
        protected (Expression<Func<R, E>> when, E then) When<E>(Expression<Func<R, E>> when, E then)
        {
            return (when, then);
        }

        /// <summary>
        /// Creates a new model associated to sensor model invoking given action
        /// </summary>
        /// <param name="update">An update action to create specific properties within model</param>
        /// <returns></returns>
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

        /// <summary>
        /// Verify expected subscription to match actual status followed occurence match for used mocks
        /// </summary>
        /// <param name="expectedStatus">An expected subscription status</param>
        /// <param name="actualStatus">An actual subscription status</param>
        /// <param name="bandClientOccurred">Occurence of band client expression mock</param>
        /// <param name="sensorManagerOccurred">Occurrence of sensor manager expression mock</param>
        /// <param name="getConsentOccurred">Occurence of get user consent expression mock</param>
        /// <param name="requestConsentOccurred">Occurence of request consent expression mock</param>
        /// <param name="startReadingOccurred">Occurrence of start reading async expression mock</param>
        protected void VerifySubscribe(bool expectedStatus, bool actualStatus, Occurred bandClientOccurred, Occurred sensorManagerOccurred, Occurred getConsentOccurred, Occurred requestConsentOccurred, Occurred startReadingOccurred)
        {
            Assert.AreEqual(expectedStatus, actualStatus, "Expected subscription status should match actual");

            bandClientService.VerifyGet(bandClientService => bandClientService.BandClient, bandClientOccurred);
            sensorManager.VerifyGet(sensorExpression, sensorManagerOccurred);
            sensor.Verify(sensor => sensor.GetCurrentUserConsent(), getConsentOccurred);
            sensor.Verify(sensor => sensor.RequestUserConsentAsync(), requestConsentOccurred);
            sensor.Verify(sensor => sensor.StartReadingsAsync(), startReadingOccurred);
        }

        /// <summary>
        /// Verify given expected model to match with sensor value changed to actual model followed by occurence match for used mocks
        /// </summary>
        /// <param name="expectedModel">An expeted model to verify</param>
        protected void VerifySensorValueChanged(T expectedModel)
        {
            Assert.AreEqual(expectedModel.ToString(), viewModel.Model.ToString(), "Expected serialized model should match actual model");

            subjectViewService.VerifyGet(subjectViewService => subjectViewService.CurrentView, Occurred.Once());
            subjectViewService.VerifyGet(subjectViewService => subjectViewService.SubjectId, Occurred.Once());
            ntpSyncService.VerifyGet(ntpSyncService => ntpSyncService.LocalTimeNow, Occurred.Once());
            subjectViewService.VerifyGet(subjectViewService => subjectViewService.SessionInProgress, Occurred.Once());
            logger.Verify(logger => logger.Information($"{{{viewModel.SensorType.GetName()}}}", Param.IsAny<T>()), Occurred.Once());
        }
    }
}
