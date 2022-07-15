/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Formatter;
using IDEASLabUT.MSBandWearable.Service;

using Serilog;

using System;
using System.IO;

using Windows.Storage;

using static IDEASLabUT.MSBandWearable.ElasticsearchLoggerGlobals;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// A utility class for configuring serilog
    /// </summary>
    internal static class SerilogLoggerUtil
    {
        private static readonly Lazy<LoggerConfiguration> LoggerConfigurationInstance;
        private static readonly Lazy<ILogger> LoggerInstance;

        static SerilogLoggerUtil()
        {
            var propertiesService = PropertiesService.Singleton;
            LoggerConfigurationInstance = new Lazy<LoggerConfiguration>(() =>
            {
                return new LoggerConfiguration().WriteTo.DurableHttpUsingFileSizeRolledBuffers(
                    requestUri: propertiesService.GetProperty(ElasticsearchUriJsonKey),
                    bufferBaseFileName: Path.Combine(ApplicationData.Current.LocalFolder.Path, propertiesService.GetProperty(LoggerFileUriJsonKey)),
                    logEventsInBatchLimit: 200,
                    textFormatter: new ElasticsearchEventJsonFormatter(null),
                    batchFormatter: new ElasticsearchBatchEventFormatter(null),
                    httpClient: ElasticsearchLoggerHttpClient.Singleton,
                    period: TimeSpan.FromSeconds(8),
                    configuration: propertiesService.GetProperties
                );
            });

            LoggerInstance = new Lazy<ILogger>(() => LoggerFactory.CreateLogger());
        }

        /// <summary>
        /// Gets the instantiated lazy singleton logger configuration instance
        /// </summary>
        private static LoggerConfiguration LoggerFactory => LoggerConfigurationInstance.Value;

        /// <summary>
        /// Gets the instantiated lazy singleton logger instance
        /// </summary>
        internal static ILogger Logger => LoggerInstance.Value;
    }
}
