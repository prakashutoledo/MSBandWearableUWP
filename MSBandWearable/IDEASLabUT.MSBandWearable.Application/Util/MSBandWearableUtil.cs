using IDEASLabUT.MSBandWearable.Application.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace IDEASLabUT.MSBandWearable.Application.Util
{
    public sealed class MSBandWearableUtil
    {
        public static async Task RunLaterInUIThread(Action action, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal)
        {
            if (action == null)
            {
                return;
            }

            CoreDispatcher coreDispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            if (coreDispatcher.HasThreadAccess)
            {
                action.Invoke();
            }
            else
            {
                await coreDispatcher.RunAsync(coreDispatcherPriority, new DispatchedHandler(action)).AsTask().ConfigureAwait(false);
            }
        }

        public static async Task RunLaterInUIThread<T>(Action<T> action, T inputValue, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal) where T : BaseEvent
        {
            await RunLaterInUIThread(() => action.Invoke(inputValue), coreDispatcherPriority).ConfigureAwait(false);
        }

        private MSBandWearableUtil()
        {
            // private initialization
        }
    }
}