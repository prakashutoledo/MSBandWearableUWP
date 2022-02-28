﻿using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Serilog;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// Provides a sensor manager basis for all available Microsoft Band 2 sensors. If the sensor is
    /// subscribed, this will provides a handler base of sensor value changed. This will also notifies 
    /// to all the listeners for as model value changes
    /// </summary>
    /// <typeparam name="T">A parameter of type <see cref="BaseEvent"/></typeparam>
    public class BaseSensorModel<T> : BaseModel where T : BaseEvent
    {
        private T model;
        protected readonly ILogger logger;
        protected readonly ISubjectViewService subjectViewService;
        protected readonly INtpSyncService ntpSyncService;
        protected readonly IBandClientService msBandService;

        public BaseSensorModel(T model, ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService)
        {
            Model = model;
            this.logger = logger;
            this.msBandService = msBandService;
            this.subjectViewService = subjectViewService;
            this.ntpSyncService = ntpSyncService;
        }

        /// <summary>
        /// An asynchronous task delegate for notifying listener that value has been changed.
        /// </summary>
        /// <param name="value">An underlying value of type <code>BaseEvent</code>that has been changed</param>
        /// <returns>A task that can be complete or can be handled later</returns>
        public delegate Task SensorValueChangedHandler(T value);
        
        public T Model
        {
            get => model;
            protected set => UpdateAndNotify(ref model, value);
        }

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
