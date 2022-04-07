using static IDEASLabUT.MSBandWearable.Application.MSBandWearableApplicationGlobals;

using IDEASLabUT.MSBandWearable.Core.Model.Elasticsearch;
using IDEASLabUT.MSBandWearable.Application.Service;

using Microsoft.Extensions.Configuration;

using Serilog;
using System;
using System.IO;

using Windows.Storage;
using Windows.ApplicationModel;

namespace IDEASLabUT.MSBandWearable.Application.Util
{
    /// <summary>
    /// Utility class for creating lazy singleton application properties and logger configuration for MS Band Wearable Application
    /// </summary>
    public static class MSBandWearableUtil
    {
        private static readonly Lazy<IConfiguration> ApplicationPropertiesInstance;
        private static readonly Lazy<LoggerConfiguration> LoggerConfigurationInstance;
        private static readonly Lazy<ILogger> LoggerInstance;

        /// <summary>
        /// Static constructor for <see cref="MSBandWearableUtil"/>
        /// </summary>
        static MSBandWearableUtil()
        {
            // Add passwords, api keys, and secret keys in ApplicationProperties.local.json (local application properties) so that 
            // it won't get uploaded in remote git repositories. Properties values inside local file are ignored by git while commiting change
            // and local to user local repositories

            // Properties inside local properties files will override the value of the property which is in ApplicationProperties.json file
            // So, please be careful in using that
            ApplicationPropertiesInstance = new Lazy<IConfiguration>(() =>
            {
                return new ConfigurationBuilder()
                    .SetBasePath(Package.Current.InstalledLocation.Path)
                    .AddJsonFile(ApplicationPropertiesFileName, optional: false, reloadOnChange: false)
                    // local application properties which is optional
                    .AddJsonFile(LocalApplicationPropertiesFileName, optional: true, reloadOnChange: false)
                    .Build();
            });

            LoggerConfigurationInstance = new Lazy<LoggerConfiguration>(() =>
            {
                return new LoggerConfiguration().WriteTo.DurableHttpUsingFileSizeRolledBuffers(
                    requestUri: ApplicationProperties.GetValue<string>(ElasticsearchUriJsonKey),
                    bufferBaseFileName: Path.Combine(ApplicationData.Current.LocalFolder.Path, ApplicationProperties.GetValue<string>(LoggerFileUriJsonKey)),
                    batchPostingLimit: 30,
                    textFormatter: new ElasticsearchEventJsonFormatter(),
                    batchFormatter: new ElasticsearchBatchEventFormatter(null),
                    httpClient: new ElasticsearchService(ApplicationProperties),
                    period: TimeSpan.FromSeconds(8)
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
        public static ILogger Logger => LoggerInstance.Value;

        /// <summary>
        /// Gets the instiated lazy singleton application properties instance
        /// </summary>
        public static IConfiguration ApplicationProperties => ApplicationPropertiesInstance.Value;
    }
}