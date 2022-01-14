﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace IDEASLabUT.MSBandWearable.Application.Util
{
    public class MSBandWearableUtil
    {
        public async Task RunLaterInUIThread(Action action = null, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal)
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
    }
}
