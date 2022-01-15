﻿using IDEASLabUT.MSBandWearable.Application.Model;
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
        public BaseSensorModel(T model) => Model = model;

        /// <summary>
        /// An asynchronous task delegate for notifying listener that value has been changed.
        /// </summary>
        /// <param name="value">An underlying value of type <code>BaseEvent</code>that has been changed</param>
        /// <returns>A task that can be complete or can be handled later</returns>
        public delegate Task SensorValueChangedHandler(T value);

        private T model;
        public T Model
        {
            get => model;
            // Notifies changes for all the property
            protected set => UpdateAndNotify(ref model, value);
        }

        /// <summary>
        /// A virtual task that can be subscribed by its corresponding sensor subclasses.
        /// Currently, it will just returns the task that is already completed
        /// </summary>
        /// <returns>A completed subscribing task</returns>
        public virtual Task Subscribe()
        {
            return Task.CompletedTask;
        }
    }
}