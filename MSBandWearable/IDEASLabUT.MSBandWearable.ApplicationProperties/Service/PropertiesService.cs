﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.Extensions.Configuration;

using System;

using Windows.ApplicationModel;

using static IDEASLabUT.MSBandWearable.ApplicationPropertiesGlobals;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Service class which helps to provide application properties value based on the provided key
    /// </summary>
    public sealed class PropertiesService : IPropertiesService
    {
        private static readonly Lazy<IPropertiesService> PropertiesServiceInstance;

        static PropertiesService()
        {
            PropertiesServiceInstance = new Lazy<IPropertiesService>(() => new PropertiesService(PropertiesConfigurationBuilder()));
        }

        internal static IPropertiesService Singleton => PropertiesServiceInstance.Value;

        private static IConfiguration PropertiesConfigurationBuilder()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Package.Current.InstalledLocation.Path) // This is a key here, UWP appx package location
                .AddJsonFile(path: ApplicationPropertiesFileName, optional: false, reloadOnChange: false)
                .AddJsonFile(path: LocalApplicationPropertiesFileName, optional: true, reloadOnChange: false)
                .Build();
        }

        private readonly IConfiguration configuration;

        /// <summary>
        /// Creates a new instance of <see cref="PropertiesService"/>
        /// </summary>
        /// <param name="configuration">A configuration to set</param>
        private PropertiesService(IConfiguration configuration)
        {
            // private initialization
            this.configuration = configuration;
        }

        public IConfiguration GetProperties => configuration;

        /// <summary>
        /// Get the value for given property key
        /// </summary>
        /// <param name="key">A key to look property value</param>
        /// <returns>A value assciated to given key if exists otherwise null</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public string GetProperty(string key)
        {
            return string.IsNullOrWhiteSpace(key) ? throw new ArgumentNullException(nameof(key)) : configuration.GetSection(key)?.Value;
        }
    }
}
