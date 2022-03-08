﻿using static IDEASLabUT.MSBandWearable.Application.MSBandWearableApplicationGlobals;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Model.Elasticsearch;
using IDEASLabUT.MSBandWearable.Application.Service;

using Microsoft.Extensions.Configuration;

using Serilog;
using System;
using System.Threading.Tasks;
using System.IO;

using Windows.UI.Core;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using System.Diagnostics;

namespace IDEASLabUT.MSBandWearable.Application.Util
{
    public static class MSBandWearableUtil
    {
        private static readonly Lazy<IConfiguration> ApplicationPropertiesInstance;
        private static readonly Lazy<LoggerConfiguration> LoggerConfigurationInstance;

        static MSBandWearableUtil()
        {
            ApplicationPropertiesInstance = new Lazy<IConfiguration>(() =>
            {
                return new ConfigurationBuilder()
                    .SetBasePath(Package.Current.InstalledLocation.Path)
                    .AddJsonFile(ApplicationPropertiesFileName, optional: false, reloadOnChange: false)
                    .AddJsonFile(ApplicationPropertiesLocalFileName, optional: true, reloadOnChange: false)
                    .Build();
            });

            LoggerConfigurationInstance = new Lazy<LoggerConfiguration>(() =>
            {
                return new LoggerConfiguration().WriteTo.DurableHttpUsingFileSizeRolledBuffers(
                    requestUri: ApplicationProperties.GetValue<string>(ElasticsearchUriJsonKey),
                    bufferBaseFileName: Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationProperties.GetValue<string>(LoggerFileUriJsonKey)),
                    batchPostingLimit: 20,
                    textFormatter: new ElasticsearchEventJsonFormatter(),
                    batchFormatter: new ElasticsearchBatchEventFormatter(null),
                    httpClient: new ElasticsearchService(ApplicationProperties),
                    period: TimeSpan.FromSeconds(5)
                );
            });

            Trace.WriteLine(ApplicationData.Current.LocalFolder.Path);
        }

        public static LoggerConfiguration LoggerFactory => LoggerConfigurationInstance.Value;
        public static IConfiguration ApplicationProperties => ApplicationPropertiesInstance.Value;

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