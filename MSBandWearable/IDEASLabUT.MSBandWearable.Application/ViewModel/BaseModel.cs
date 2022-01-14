using IDEASLabUT.MSBandWearable.Application.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    [DataContract]
    public class BaseModel<T>: INotifyPropertyChanged where T: BaseEvent
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void ValueChangedChangedHandler(double force);
        public event ValueChangedChangedHandler ValueChanged;

        public T Value { get; set; }

        /// <summary>
        /// Update the given target value (replacing the refrence) if the value has really changed.
        /// This will also notify property changed event for the given property name. There is no need
        /// for providing explicit property name from the caller as property is determined dynamically
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual void UpdateAndNotify<T>(ref T target, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(target, value))
            {
                return;
            }

            target = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
