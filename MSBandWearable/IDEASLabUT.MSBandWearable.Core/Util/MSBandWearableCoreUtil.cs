using IDEASLabUT.MSBandWearable.Core.Model;

using System;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.ApplicationModel.Core;

namespace IDEASLabUT.MSBandWearable.Core.Util
{
    public static class MSBandWearableCoreUtil
    {
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
            }
            else
            {
                await coreDispatcher.RunAsync(coreDispatcherPriority, new DispatchedHandler(action));
            }
        }

        public static async Task RunLaterInUIThread<T>(Action<T> action, T inputValue, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal) where T : BaseEvent
        {
            await RunLaterInUIThread(() => action.Invoke(inputValue), coreDispatcherPriority);
        }
    }
}
