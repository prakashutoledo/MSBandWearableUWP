using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using Serilog;
using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// Provides a sensor manager basis for all available Microsoft Band 2 sensors. If the sensor is
    /// subscribed, this will provides a handler base of sensor value changed. This will also notifies 
    /// to all the listeners for as model value changes
    /// </summary>
    /// <typeparam name="T">A parameter of type <see cref="BaseEvent"/></typeparam>
    public abstract class BaseSensorModel<T, R> : BaseModel where T : BaseEvent where R : IBandSensorReading
    {
        private T model;
        protected readonly ILogger logger;
        protected readonly ISubjectViewService subjectViewService;
        protected readonly INtpSyncService ntpSyncService;
        protected readonly IBandClientService msBandService;
        /// <summary>
        /// An asynchronous task function for notifying listener that sensor value has been changed.
        /// </summary>
        /// <param name="value">An underlying value of type <code>BaseEvent</code>that has been changed</param>
        /// <returns>A task that can be complete or can be handled later</returns>
        public Func<T, Task> SensorValueChanged { get; set; }

        public BaseSensorModel(T model, ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService)
        {
            Model = model;
            this.logger = logger;
            this.msBandService = msBandService;
            this.subjectViewService = subjectViewService;
            this.ntpSyncService = ntpSyncService;
        }

        public T Model
        {
            get => model;
            protected set => UpdateAndNotify(ref model, value);
        }

        /// <summary>
        /// Updates the given sensor to include sensor reading value changed handler for given action 
        /// </summary>
        /// <param name="sensor">A MS Band sensor to handle the given action</param>
        /// <param name="sensorReadingChanged">A reading changed action for handling sensor value reading</param>
        public abstract void UpdateSensorReadingChangedHandler(IBandSensor<R> sensor, Action<R> sensorReadingChanged);

        /// <summary>
        /// A virtual task that can be subscribed by its corresponding sensor subclasses.
        /// Currently, it will just returns the task that is already completed
        /// </summary>
        /// <returns>A completed subscribing task</returns>
        public virtual async Task Subscribe()
        {
            await Task.CompletedTask;
        }
    }
}
