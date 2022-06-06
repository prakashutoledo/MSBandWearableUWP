using static IDEASLabUT.MSBandWearable.Util.MSBandWearableCoreUtil;

using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Service;

using Microsoft.Band;
using Microsoft.Band.Sensors;

using Serilog;

using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Provides a sensor manager basis for all available Microsoft Band 2 sensors. If the sensor is
    /// subscribed, this will provides a handler base of sensor value changed. This will also notifies 
    /// to all the listeners for as model value changes. Model is initialized only during construction and 
    /// values are updated to existing model instead of creating new instance of model when value is changed
    /// </summary>
    /// <typeparam name="SensorEvent">A parameter of type <see cref="BaseEvent"/></typeparam>
    /// <typeparam name="SensorReading">A parameter of type <see cref="IBandSensorReading"/></typeparam>
    public abstract class BaseSensorViewModel<SensorEvent, SensorReading> : BaseViewModel where SensorEvent : BaseEvent, new() where SensorReading : IBandSensorReading
    {
        private SensorEvent model;
        private readonly SensorType sensorType;
        private readonly ILogger logger;
        private readonly ISubjectViewService subjectViewService;
        private readonly INtpSyncService ntpSyncService;
        private readonly IBandClientService msBandService;
        private readonly Action<SensorReading> updateSensorModel;
        private readonly Func<IBandSensorManager, IBandSensor<SensorReading>> bandSensorSupplier;

        /// <summary>
        /// Initializes a new instance of <see cref="BaseSensorViewModel{T, R}"/>
        /// </summary>
        /// <param name="sensorType">A type of this sensor to set</param>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp synchronization to set</param>
        /// <exception cref="ArgumentNullException">If any of the parameters model, logger, msBandService, subjectViewService or ntpSyncService is null</exception>
        protected BaseSensorViewModel(SensorType sensorType, ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService)
        {
            this.sensorType = sensorType;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.msBandService = msBandService ?? throw new ArgumentNullException(nameof(msBandService));
            this.subjectViewService = subjectViewService ?? throw new ArgumentNullException(nameof(subjectViewService));
            this.ntpSyncService = ntpSyncService ?? throw new ArgumentNullException(nameof(ntpSyncService));
            Model = new SensorEvent();
        }

        /// <summary>
        /// An asynchronous task function for notifying listener that sensor value has been changed.
        /// </summary>
        /// <param name="value">A value to set</param>
        /// <returns>A task that can be awaited</returns>
        public Func<SensorEvent, Task> SensorModelChanged { get; set; }

        /// <summary>
        /// A current sensor model holding data for MS Band 2 sensor
        /// </summary>
        public SensorEvent Model
        {
            get => model;
            protected set => UpdateAndNotify(ref model, value);
        }

        /// <summary>
        /// A current sensor type
        /// </summary>
        public SensorType SensorType
        {
            get => sensorType;
        }

        /// <summary>
        /// Gets the MS band 2 sensor from given band sensor manager
        /// </summary>
        /// <param name="sensorManager">A sensor manager to be used to get the band sensor</param>
        /// <returns>The corresponding MS band 2 sensor</returns>
        protected abstract IBandSensor<SensorReading> GetBandSensor(IBandSensorManager sensorManager);

        /// <summary>
        /// A callback for a change in MS band 2 sensor reading
        /// </summary>
        /// <param name="sensorReading">A current sensor value reading for the corresponding sensor</param>
        /// <remarks>This function is guaranteed to run in a Core Dispatcher thread. Thus implementing sub class doesn't need to update model in Dispatcher thread</remarks>
        protected abstract void UpdateSensorModel(SensorReading sensorReading);

        /// <summary>
        /// A task that can be subscribe sensor to start reading values by setting callback. This will 
        /// request the current user consent for subscribing corresponding sensor, if such request is not
        /// granted a sensor is not suscribed and will not start reading the changed values.
        /// </summary>
        /// <returns>A task that can be awaited which determines if we can read sensor</returns>
        public async Task<bool> Subscribe()
        {
            var sensorManager = msBandService.BandClient.SensorManager;
            if (sensorManager == null)
            {
                return false;
            }

            var sensor = GetBandSensor(sensorManager);
            var userConsent = sensor.GetCurrentUserConsent() == UserConsent.Granted || await sensor.RequestUserConsentAsync();
            if (!userConsent)
            {
                return false;
            }

            sensor.ReadingChanged += OnBandSensorReadingChanged;
            var startReadingStatus =  await sensor.StartReadingsAsync();
            return startReadingStatus;
        }

        /// <summary>
        /// A callback for subscribing MS Band 2 senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">A band sensor reading event arguments</param>
        /// <see cref="BandSensorReadingEventArgs{R}"/>
        private async void OnBandSensorReadingChanged(object sender, BandSensorReadingEventArgs<SensorReading> readingEventArgs)
        {
            var sensorReading = readingEventArgs.SensorReading;
            if (sensorReading == null)
            {
                return;
            }

            // These next 4 lines for Model are not used in UI. So, they don't need to run in core dispatcher UI
            // Thread to update their values. They are only used for logger
            Model.FromView = subjectViewService.CurrentView;
            Model.AcquiredTime = ntpSyncService.LocalTimeNow;
            Model.ActualTime = sensorReading.Timestamp.DateTime;
            Model.SubjectId = subjectViewService.SubjectId;

            // This update sensor model should reflect change in UI. Thus, it should update model in core dispatcher UI thread
            await RunLaterInUIThread(() =>
            {
                UpdateSensorModel(sensorReading);
                NotifyPropertyChanged(nameof(Model));
            });

            if (subjectViewService.SessionInProgress)
            {
                logger.Information($"{{{sensorType.GetName()}}}", Model);
            }

            if (SensorModelChanged != null)
            {
                await SensorModelChanged.Invoke(Model);
            }
        }
    }
}
