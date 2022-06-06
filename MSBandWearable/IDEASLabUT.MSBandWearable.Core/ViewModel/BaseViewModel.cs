using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// Base class for all view model which listens property changed for all changed properties
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A property changed event handler delegate
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the listener that property has been changed. Property name is optional
        /// as it can be provided dynamically by compilers supporting <see cref="CallerMemberNameAttribute"/>
        /// </summary>
        /// <param name="propertyName">An name of the property that has been changed.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Update the given target value (replacing the refrence) if the value has really changed.
        /// This will also notify property changed event for the given property name. There is no need
        /// for providing explicit property name from the caller as property is determined dynamically
        /// </summary>
        /// <typeparam name="T">A type of the property</typeparam>
        /// <param name="target">A target reference value of the property being changed</param>
        /// <param name="value">A new value of the property being changed</param>
        /// <param name="propertyName">An optional property name that is going to be changed</param>
        /// <returns><code>true</code> if updated otherwise <code>false</code></returns>
        protected virtual bool UpdateAndNotify<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(target, value))
            {
                return false;
            }
            target = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
