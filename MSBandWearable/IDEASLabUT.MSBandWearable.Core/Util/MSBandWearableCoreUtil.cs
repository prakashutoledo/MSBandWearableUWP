using IDEASLabUT.MSBandWearable.Core.Model;

using System;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.ApplicationModel.Core;

namespace IDEASLabUT.MSBandWearable.Core.Util
{
    /// <summary>
    /// Core utility class for runnining action in core dispatcher main thread for UI components changed values
    /// </summary>
    public static class MSBandWearableCoreUtil
    {
        /// <summary>
        /// Runs the given priority action in main core dispatcher thread asynchronously. Null action will not be invoked in core dispatcher thread
        /// </summary>
        /// <param name="action">An action to be invoked in core dispatcher thread</param>
        /// <param name="coreDispatcherPriority">A core dispatcher priority for invoked action which defaults to normal</param>
        /// <returns>A task that can be awaited</returns>
        public static async Task RunLaterInUIThread(Action action, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal)
        {
            if (action == null)
            {
                return;
            }

            var coreDispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            if (coreDispatcher.HasThreadAccess)
            {
                action.Invoke();
                return;
            }
            await coreDispatcher.RunAsync(coreDispatcherPriority, new DispatchedHandler(action));
        }

        /// <summary>
        /// Runs the given priority action in main core dispatcher thread asynchronously by passing given input value
        /// </summary>
        /// <typeparam name="T">A parameter of type <see cref="BaseEvent"/> </typeparam>
        /// <param name="action">An action to be invoked in core dispatcher thread</param>
        /// <param name="inputValue">An input value for given action</param>
        /// <param name="coreDispatcherPriority">A core dispatcher priority for invoked action which defaults to normal</param>
        /// <returns>A task that can be awaited</returns>
        public static async Task RunLaterInUIThread<T>(Action<T> action, T inputValue, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal) where T : BaseEvent
        {
            await RunLaterInUIThread(() => action.Invoke(inputValue), coreDispatcherPriority);
        }
    }
}
