using static IDEASLabUT.MSBandWearable.Application.MSBandWearableApplicationGlobals;

using IDEASLabUT.MSBandWearable.Application.Model.Elasticsearch;
using IDEASLabUT.MSBandWearable.Application.Service;

using Microsoft.Extensions.Configuration;

using Serilog;
using System;
using System.IO;

using Windows.Storage;
using Windows.ApplicationModel;
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
    }
}