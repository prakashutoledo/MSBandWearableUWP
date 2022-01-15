using IDEASLabUT.MSBandWearable.Application.Model;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    public class BaseSensorModel<T> : BaseModel where T : BaseEvent
    {
        /// <summary>
        /// An asynchronous task delegate for notifying listener that value has been changed.
        /// </summary>
        /// <param name="value">An underlying value that has been changed</param>
        /// <returns></returns>
        public delegate void SensorValueChangedHandler(T value);

        private T data;
        public T Data
        {
            get => data;
            // Notifies changes for all the property
            set => UpdateAndNotify(ref data, value, string.Empty);
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
