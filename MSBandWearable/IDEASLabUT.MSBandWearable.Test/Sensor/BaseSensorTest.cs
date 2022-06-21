using HyperMock;

using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Serilog;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using static HyperMock.Occurred;
using static Microsoft.Band.UserConsent;

namespace IDEASLabUT.MSBandWearable.Sensor
{
    /// <summary>
    /// Base sensor model test for all view sensor models inheriting <see cref="BaseSensor{SensorEvent, SensorReading}"/>
    /// </summary>
    /// <typeparam name="SensorEvent">A parameter of type <see cref="BaseEvent"/></typeparam>
    /// <typeparam name="SensorReading">A parameter of type <see cref="IBandSensorReading"/></typeparam>
    /// <typeparam name="Sensor">A parameter of type inheriting <see cref="BaseSensor{SensorEvent, SensorReading}"/></typeparam>
    [TestClass]
    public class BaseSensorTest<SensorEvent, SensorReading, Sensor> : BaseHyperMock<Sensor> where SensorEvent : BaseEvent, new() where SensorReading : IBandSensorReading where Sensor : BaseSensor<SensorEvent, SensorReading>
    {
        private readonly Expression<Func<IBandSensorManager, IBandSensor<SensorReading>>> sensorExpression;

        /// <summary>
        /// Creates a new instance of <see cref="BaseSensorTest{SensorEvent, SensorReading, Sensor}"/>
        /// </summary>
        /// <param name="sensorExpression">An expression for getting sensor from sensor manager</param>
        protected BaseSensorTest(Expression<Func<IBandSensorManager, IBandSensor<SensorReading>>> sensorExpression)
        {
            this.sensorExpression = sensorExpression ?? throw new ArgumentNullException(nameof(sensorExpression));
        }

        [TestInitialize]
        public void Initialize()
        {
            MockFor<IBandSensorManager>(sensorManagerMock => sensorManagerMock.SetupGet(sensorExpression).Returns(MockValue<IBandSensor<SensorReading>>()));
            MockFor<IBandClient>(bandClientMock => bandClientMock.SetupGet(bandClient => bandClient.SensorManager).Returns(MockValue<IBandSensorManager>()));
            MockFor<IBandClientService>(bandClientServiceMock => bandClientServiceMock.SetupGet(bandClientService => bandClientService.BandClient).Returns(MockValue<IBandClient>()));
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
            MockFor<SensorReading>(sensorReading =>
            {
                sensorReading.SetupGet(first.when).Returns(first.then);
                sensorReading.SetupGet(second.when).Returns(second.then);
            });

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
            MockFor<SensorReading>(sensorReadingMock =>
            {
                sensorReadingMock.SetupGet(first.when).Returns(first.then);

                foreach (var (when, then) in remaining)
                {
                    sensorReadingMock.SetupGet(when).Returns(then);
                }
            });

            await MockSensorReadingChanged();
        }

        /// <summary>
        /// Mocks sensor reading changed handler by seting all setup for mocking model value changed
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        private async Task MockSensorReadingChanged()
        {
            Subject.SensorModelChanged = () =>
            {
                ApplyLatch();
                return Task.CompletedTask;
            };
            _ = await MockSubscribe();
            MockFor<SensorReading>(sensorReadingMock => sensorReadingMock.SetupGet(sensorReading => sensorReading.Timestamp).Returns(Param.IsAny<DateTime>()));
            MockFor<ISubjectViewService>(subjectViewServiceMock =>
            {
                _ = subjectViewServiceMock.SetupGet(subjectViewService => subjectViewService.CurrentView).Returns("Fake View");
                _ = subjectViewServiceMock.SetupGet(subjectViewService => subjectViewService.SubjectId).Returns("Fake Id");
                subjectViewServiceMock.SetupGet(subjectViewService => subjectViewService.SessionInProgress).Returns(true);
            });
            MockFor<INtpSyncService>(ntpSyncServiceMock => ntpSyncServiceMock.SetupGet(ntpSyncService => ntpSyncService.LocalTimeNow).Returns(Param.IsAny<DateTime>()));

            // Raise Sensor reading change event
            MockFor<IBandSensor<SensorReading>>(sensorMock => sensorMock.Raise(sensor => sensor.ReadingChanged += null, new BandSensorReadingEventArgs<SensorReading>(MockValue<SensorReading>())));
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
            MockFor<IBandSensor<SensorReading>>(sensorMock =>
            {
                _ = sensorMock.Setup(sensor => sensor.GetCurrentUserConsent()).Returns(currentUserConsent);
                _ = sensorMock.Setup(sensor => sensor.RequestUserConsentAsync()).Returns(Task.FromResult(requestUserAsync));
                _ = sensorMock.Setup(sensor => sensor.StartReadingsAsync()).Returns(Task.FromResult(startReadingAsync));
            });

            return await Subject.Subscribe();
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
            MockFor<IBandClientService>(bandClientServiceMock => bandClientServiceMock.VerifyGet(bandClientService => bandClientService.BandClient, bandClientOccurred));
            MockFor<IBandSensorManager>(sensorManagerMock => sensorManagerMock.VerifyGet(sensorExpression, sensorManagerOccurred));
            MockFor<IBandSensor<SensorReading>>(sensorMock =>
            {
                sensorMock.Verify(sensor => sensor.GetCurrentUserConsent(), getConsentOccurred);
                sensorMock.Verify(sensor => sensor.RequestUserConsentAsync(), requestConsentOccurred);
                sensorMock.Verify(sensor => sensor.StartReadingsAsync(), startReadingOccurred);
            });

        }

        /// <summary>
        /// Verify given expected model to match with sensor value changed to actual model followed by occurence match for used mocks
        /// </summary>
        /// <param name="expectedModel">An expeted model to verify</param>
        protected void VerifySensorValueChanged(SensorEvent expectedModel)
        {
            Assert.AreEqual(expectedModel.ToString(), Subject.Model.ToString(), "Expected serialized model should match actual model");
            MockFor<ISubjectViewService>(subjectViewServiceMock =>
            {
                subjectViewServiceMock.VerifyGet(subjectViewService => subjectViewService.CurrentView, AtLeast(1));
                subjectViewServiceMock.VerifyGet(subjectViewService => subjectViewService.SubjectId, AtLeast(1));
                subjectViewServiceMock.VerifyGet(subjectViewService => subjectViewService.SessionInProgress, AtLeast(1));
            });

            MockFor<INtpSyncService>(ntpSyncServiceMock => ntpSyncServiceMock.VerifyGet(ntpSyncService => ntpSyncService.LocalTimeNow, AtLeast(1)));
            MockFor<ILogger>(loggerMock => loggerMock.Verify(logger => logger.Information($"{{{Subject.SensorType.GetName()}}}", Param.IsAny<SensorEvent>()), AtLeast(1)));
        }
    }
}
