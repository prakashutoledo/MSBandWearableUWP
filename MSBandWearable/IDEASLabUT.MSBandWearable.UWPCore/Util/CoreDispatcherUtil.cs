/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// Core dispatcher thread utility class to provide action to run in core dispatcher thread
    /// </summary>
    public class CoreDispatcherUtil
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
        public static Task RunLaterInUIThread<T>(Action<T> action, T inputValue, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal)
        {
            return RunLaterInUIThread(() => action.Invoke(inputValue), coreDispatcherPriority);
        }
    }
}
