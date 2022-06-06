using HyperMock;

using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;
using IDEASLabUT.MSBandWearable.Core.ViewModel;

using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using static HyperMock.Occurred;
using static Microsoft.Band.UserConsent;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    /// <summary>
    /// Base sensor model test for all view sensor models inheriting <see cref="BaseSensorViewModel{T, R}"/>
    /// </summary>
    /// <typeparam name="T">A parameter of type <see cref="BaseEvent"/></typeparam>
    /// <typeparam name="R">A parameter of type <see cref="IBandSensorReading"/></typeparam>
    [TestClass]
    public class BaseSensorTest<T, R> : BaseViewModelTest where T : BaseEvent, new() where R : IBandSensorReading
    {
        private Mock<ILogger> logger;
        private Mock<IBandClientService> bandClientService;
        private Mock<ISubjectViewService> subjectViewService;
        private Mock<INtpSyncService> ntpSyncService;
        private Mock<IBandClient> band;
        private Mock<IBandSensorManager> sensorManager;
        private Mock<R> sensorReading;
        private Mock<IBandSensor<R>> sensor;
        private BaseSensorViewModel<T, R> viewModel;

        private readonly Expression<Func<IBandSensorManager, IBandSensor<R>>> sensorExpression;
        private readonly Func<ILogger, IBandClientService, ISubjectViewService, INtpSyncService, BaseSensorViewModel<T, R>> viewModelSupplier;
        private readonly IDictionary<string, int> propertyMap;

        protected BaseSensorTest(Expression<Func<IBandSensorManager, IBandSensor<R>>> sensorExpression, Func<ILogger, IBandClientService, ISubjectViewService, INtpSyncService, BaseSensorViewModel<T, R>> viewModelSupplier)
        {
            this.sensorExpression = sensorExpression ?? throw new ArgumentNullException(nameof(sensorExpression));
            this.viewModelSupplier = viewModelSupplier ?? throw new ArgumentNullException(nameof(viewModelSupplier));
            propertyMap = new Dictionary<string, int>();
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

            viewModel = viewModelSupplier.Invoke(logger.Object, bandClientService.Object, subjectViewService.Object, ntpSyncService.Object);
            viewModel.PropertyChanged += OnPropertyChanged;
        }

        [TestMethod]
        public async Task SubscribeSensorWithCurrentUserConsentSuccess()
        {
            var actualStatus = await MockSubscribe();
            VerifySubscribe(true, actualStatus, Once(), Once(), Once(), Never(), Once());
        }


        [TestMethod]
        public async Task SubscribeSensorWithRequestUserConsentSuccess()
        {
            var actualStatus = await MockSubscribe(currentUserConsent: NotSpecified, requestUserAsync: true);
            VerifySubscribe(true, actualStatus, Once(), Once(), Once(), Once(), Once());
        }

        [TestMethod]
        public async Task SubscribeSensorWithRequestUserConsentReadFailure()
        {
            var actualStatus = await MockSubscribe(currentUserConsent: Granted, startReadingAsync: false);
            VerifySubscribe(false, actualStatus, Once(), Once(), Once(), Never(), Once());
        }

        [TestMethod]
        public async Task SubscribeSensorWithRequestUserConsentNotGranted()
        {
            var actualStatus = await MockSubscribe(currentUserConsent: Declined);
            VerifySubscribe(false, actualStatus, Once(), Once(), Once(), Once(), Never());
        }

        [TestCleanup]
        public void SensorCleanup()
        {
            logger = null;
            bandClientService = null;
            subjectViewService = null;
            ntpSyncService = null;
            band = null;
            sensorManager = null;
            sensorReading = null;
            sensor = null;
            viewModel = null;
            propertyMap.Clear();
        }

        /// <summary>
        /// Mocks sensor reading changed handler by seting all setup for mocking model value changed
        /// </summary>
        /// <typeparam name="E">A type of mock return value for when expression</typeparam>
        /// <param name="setups">All tuples (when, then) expression setup for mocking model change</param>
        /// <returns></returns>
        protected async Task MockSensorReadingChanged<E>(params (Expression<Func<R, E>> when, E then)[] setups)
        {
            EventWaitHandle awaitLatch = new AutoResetEvent(false);
            viewModel.SensorModelChanged = model =>
            {
                Assert.IsNotNull(model, "Changed model shouldn't be null");
                awaitLatch.Set();
                return Task.CompletedTask;
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

            VerifyProperty(propertyName : "Model", expectedCount: 1);
        }
    }
}
