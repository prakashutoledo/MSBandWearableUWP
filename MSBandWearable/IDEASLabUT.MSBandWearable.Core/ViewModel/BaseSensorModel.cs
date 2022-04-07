﻿using static IDEASLabUT.MSBandWearable.Core.Util.MSBandWearableCoreUtil;

using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band;
using Microsoft.Band.Sensors;

using Serilog;

using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// Provides a sensor manager basis for all available Microsoft Band 2 sensors. If the sensor is
    /// subscribed, this will provides a handler base of sensor value changed. This will also notifies 
    /// to all the listeners for as model value changes
    /// </summary>
    /// <typeparam name="T">A parameter of type <see cref="BaseEvent"/></typeparam>
    /// <typeparam name="R">A parameter of type <see cref="IBandSensorReading"/></typeparam>
    public abstract class BaseSensorModel<T, R> : BaseModel where T : BaseEvent where R : IBandSensorReading
    {
        private T model;
        private readonly SensorType sensorType;
        private readonly ILogger logger;
        private readonly ISubjectViewService subjectViewService;
        private readonly INtpSyncService ntpSyncService;
        private readonly IBandClientService msBandService;

        public BaseSensorModel(SensorType sensorType, T model, ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService)
        {
            this.sensorType = sensorType;
            Model = model ?? throw new ArgumentNullException(nameof(model));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.msBandService = msBandService ?? throw new ArgumentNullException(nameof(msBandService));
            this.subjectViewService = subjectViewService ?? throw new ArgumentNullException(nameof(subjectViewService));
            this.ntpSyncService = ntpSyncService ?? throw new ArgumentNullException(nameof(ntpSyncService));
        }

        /// <summary>
        /// An asynchronous task function for notifying listener that sensor value has been changed.
        /// </summary>
        /// <param name="value">An underlying value of type <code>BaseEvent</code>that has been changed</param>
        /// <returns>A task that can be awaited</returns>
        public Func<T, Task> SensorModelChanged { get; set; }

        /// <summary>
        /// A current sensor model holding data for MS Band 2 sensor
        /// </summary>
        public T Model
        {
            get => model;
            protected set => UpdateAndNotify(ref model, value);
        }

        /// <summary>
        /// Gets the MS band 2 sensor from given sensor manager
        /// </summary>
        /// <param name="sensorManager">A sensor manager to be used to get the band sensor</param>
        /// <returns>The corresponding MS band 2 sensor</returns>
        protected abstract IBandSensor<R> GetBandSensor(IBandSensorManager sensorManager);

        /// <summary>
        /// A callback for a change in MS band 2 sensor reading
        /// </summary>
        /// <param name="sensorReading">A current sensor value reading for the corresponding sensor</param>
        protected abstract void UpdateSensorModel(R sensorReading);

        /// <summary>
        /// A task that can be subscribed by its corresponding sensor subclasses to start reading values
        /// by setting callback for reading the values. This will request the current user consent for
        /// corresponding sensor, if such request is not granted a sensor is not suscribed and will not
        /// start reading the changed values.
        /// </summary>
        /// <returns>A completed subscribing task</returns>
        public async Task Subscribe()
        {
            var sensorManager = msBandService.BandClient.SensorManager;
            if (null == sensorManager)
            {
                return;
            }

            var sensor = GetBandSensor(sensorManager);
            var userConsent = UserConsent.Granted == sensor.GetCurrentUserConsent() || await sensor.RequestUserConsentAsync();
            if (!userConsent)
            {
                return;
            }

            sensor.ReadingChanged += OnBandSensorReadingChanged;
            _ = await sensor.StartReadingsAsync();
        }

        /// <summary>
        /// A callback for subscribing MS Band 2 senser reading event changes
        /// </summary>
        /// <param name="sender">The sender of the current changed event</param>
        /// <param name="readingEventArgs">A band sensor reading event arguments</param>
        /// <see cref="BandSensorReadingEventArgs{R}"/>
        private async void OnBandSensorReadingChanged(object sendor, BandSensorReadingEventArgs<R> readingEventArgs)
        {
            var sensorReading = readingEventArgs.SensorReading;
            if (null == sensorReading)
            {
                return;
            }

            // These next 4 lines for Model are not used in UI. So, they don't need to run in UI
            // Thread to update their values. They are only used for logger
            Model.FromView = subjectViewService.CurrentView;
            Model.AcquiredTime = ntpSyncService.LocalTimeNow;
            Model.ActualTime = sensorReading.Timestamp.DateTime;
            Model.SubjectId = subjectViewService.SubjectId;

            await RunLaterInUIThread(() =>
            {
                UpdateSensorModel(readingEventArgs.SensorReading);
            });

            if (subjectViewService.SessionInProgress)
            {

                logger.Information($"{{{sensorType.GetName()}}}", Model);
            }

            if (null != SensorModelChanged)
            {
                await SensorModelChanged.Invoke(Model);
            }
        }
    }
}
