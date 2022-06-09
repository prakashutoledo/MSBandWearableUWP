using HyperMock;

using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;

using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Serilog;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;


using static HyperMock.Occurred;
using static Microsoft.Band.UserConsent;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Base sensor model test for all view sensor models inheriting <see cref="BaseSensorViewModel{SensorEvent, SensorReading}"/>
    /// </summary>
    /// <typeparam name="SensorEvent">A parameter of type <see cref="BaseEvent"/></typeparam>
    /// <typeparam name="SensorReading">A parameter of type <see cref="IBandSensorReading"/></typeparam>
    /// <typeparam name="SensorViewModel">A parameter of type inheriting <see cref="BaseSensorViewModel{T, R}"/></typeparam>
    [TestClass]
    public class BaseSensorTest<SensorEvent, SensorReading, SensorViewModel> : BaseViewModelTest where SensorEvent : BaseEvent, new() where SensorReading : IBandSensorReading where SensorViewModel : BaseSensorViewModel<SensorEvent, SensorReading>
    {
        private Mock<ILogger> logger;
        private Mock<IBandClientService> bandClientService;
        private Mock<ISubjectViewService> subjectViewService;
        private Mock<INtpSyncService> ntpSyncService;
        private Mock<IBandClient> band;
        private Mock<IBandSensorManager> sensorManager;
        private Mock<SensorReading> sensorReading;
        private Mock<IBandSensor<SensorReading>> sensor;
        private SensorViewModel viewModel;

        private readonly Expression<Func<IBandSensorManager, IBandSensor<SensorReading>>> sensorExpression;
        private readonly Func<ILogger, IBandClientService, ISubjectViewService, INtpSyncService, SensorViewModel> viewModelSupplier;

        /// <summary>
        /// Creates a new instance of <see cref="BaseSensorTest{SensorEvent, SensorReading, SensorViewModel}"/>
        /// </summary>
        /// <param name="sensorExpression">An expression for getting sensor from sensor manager</param>
        /// <param name="viewModelSupplier">A supplies for view model</param>
        protected BaseSensorTest(Expression<Func<IBandSensorManager, IBandSensor<SensorReading>>> sensorExpression, Func<ILogger, IBandClientService, ISubjectViewService, INtpSyncService, SensorViewModel> viewModelSupplier)
        {
            this.sensorExpression = sensorExpression ?? throw new ArgumentNullException(nameof(sensorExpression));
            this.viewModelSupplier = viewModelSupplier ?? throw new ArgumentNullException(nameof(viewModelSupplier));
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
            sensorReading = Mock.Create<SensorReading>();
            sensor = Mock.Create<IBandSensor<SensorReading>>();

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
        }

        /// <summary>
        /// Mocks sensor reading changed handler by seting all setup for mocking model value changed
        /// </summary>
        /// <typeparam name="X">A type of first paramater</typeparam>
        /// <typeparam name="Y">A type of second paramater</typeparam>
        /// <param name="first">A first tuple(when, then) expression setup for model changes</param>
        /// <param name="second">A second tuple(when, then) expression setup for model changes</param>
        /// <returns>A task that can be awaited</returns>
        protected async Task MockSensorReadingChanged<X, Y>((Expression<Func<SensorReading, X>> when, X then) first, (Expression<Func<SensorReading, Y>> when, Y then) second)
        {
            sensorReading.SetupGet(first.when).Returns(first.then);
            sensorReading.SetupGet(second.when).Returns(second.then);
            await MockSensorReadingChanged();
        }

        /// <summary>
        /// Mocks sensor reading changed handler by seting all setup for mocking model value changed
        /// </summary>
        /// <typeparam name="E">A type of mock return value for when expression</typeparam>
        /// <param name="first">First tuple (when, then) expression setup for mocking model changes</param>
        /// <param name="remaining">A remaining tuples (when, then) expression setup for mocking model change</param>
        /// <returns>A task that can be awaited</returns>
        protected async Task MockSensorReadingChanged<E>((Expression<Func<SensorReading, E>> when, E then) first, params (Expression<Func<SensorReading, E>> when, E then)[] remaining)
        {
            sensorReading.SetupGet(first.when).Returns(first.then);

            foreach (var setup in remaining)
            {
                sensorReading.SetupGet(setup.when).Returns(setup.then);
            }

            await MockSensorReadingChanged();
        }

        /// <summary>
        /// Mocks sensor reading changed handler by seting all setup for mocking model value changed
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        private async Task MockSensorReadingChanged()
        {
            viewModel.SensorModelChanged = async model =>
            {
                ApplyLatch(() => Assert.IsNotNull(model, "Changed model shouldn't be null"));
                await Task.CompletedTask;
            };
            _ = await MockSubscribe();
            sensorReading.SetupGet(sensorReading => sensorReading.Timestamp).Returns(Param.IsAny<DateTime>());
            subjectViewService.SetupGet(subjectViewService => subjectViewService.CurrentView).Returns("Fake View");
            subjectViewService.SetupGet(subjectViewService => subjectViewService.SubjectId).Returns("Fake Id");
            subjectViewService.SetupGet(subjectViewService => subjectViewService.SessionInProgress).Returns(true);
            ntpSyncService.SetupGet(ntpSyncService => ntpSyncService.LocalTimeNow).Returns(Param.IsAny<DateTime>());

            // Raise Sensor reading change event
            sensor.Raise(sensor => sensor.ReadingChanged += null, new BandSensorReadingEventArgs<SensorReading>(sensorReading.Object));
            // Wait for signal to be received
            WaitFor();
        }

        /// <summary>
        /// Mocks all properties for sensor subscription task
        /// </summary>
        /// <param name="currentUserConsent">A current user consent to set</param>
        /// <param name="requestUserAsync">A return value for request user async expression mock</param>
        /// <param name="startReadingAsync">A return for start reading async expression mock</param>
        /// <returns>An awaiatable task with sensor subscription status</returns>
        protected async Task<bool> MockSubscribe(UserConsent currentUserConsent = Granted, bool requestUserAsync = false, bool startReadingAsync = true)
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
        protected (Expression<Func<SensorReading, E>> when, E then) When<E>(Expression<Func<SensorReading, E>> when, E then)
        {
            return (when, then);
        }

        /// <summary>
        /// Creates a new model associated to sensor model invoking given action
        /// </summary>
        /// <param name="update">An update action to create specific properties within model</param>
        /// <returns></returns>
        protected SensorEvent NewModel(Action<SensorEvent> update)
        {
            var modelEvent = new SensorEvent();
            update?.Invoke(modelEvent);
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
        /// <param name="extraPropertyVerifier">An extra sensor model view property verifier</param>
        protected void VerifySensorValueChanged(SensorEvent expectedModel, int expectedCount = 1, Action<SensorViewModel> extraPropertyVerifier = null)
        {
            Assert.AreEqual(expectedModel.ToString(), viewModel.Model.ToString(), "Expected serialized model should match actual model");

            subjectViewService.VerifyGet(subjectViewService => subjectViewService.CurrentView, AtLeast(1));
            subjectViewService.VerifyGet(subjectViewService => subjectViewService.SubjectId, AtLeast(1));
            ntpSyncService.VerifyGet(ntpSyncService => ntpSyncService.LocalTimeNow, AtLeast(1));
            subjectViewService.VerifyGet(subjectViewService => subjectViewService.SessionInProgress, AtLeast(1));
            logger.Verify(logger => logger.Information($"{{{viewModel.SensorType.GetName()}}}", Param.IsAny<SensorEvent>()), AtLeast(1));

            VerifyProperty(propertyName : "Model", expectedCount: expectedCount);

            extraPropertyVerifier?.Invoke(viewModel);
        }
    }
}
