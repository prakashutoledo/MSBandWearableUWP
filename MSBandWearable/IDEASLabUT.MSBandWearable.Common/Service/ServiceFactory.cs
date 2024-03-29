﻿using IDEASLabUT.MSBandWearable.Util;

using Serilog;

using System;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Factory class for all service classes
    /// </summary>
    public abstract class ServiceFactory
    {
        /// <summary>
        /// Private final implementation class of <see cref="ServiceFactory"/>
        /// </summary>
        private sealed class ServiceFactoryImpl : ServiceFactory
        {
            /// <summary>
            /// Creates a new instance of <see cref="ServiceFactoryImpl"/>
            /// </summary>
            public ServiceFactoryImpl()
            {
                GetWebSocketService = WebSocketService.Singleton;
                GetPropertiesService = PropertiesService.Singleton;
                GetNtpSyncService = NtpSyncService.Singleton;
                GetSubjectViewService = SubjectViewService.Singleton;
                GetBandManagerService = MSBandManagerService.Singleton;
                GetLogger = SerilogLoggerUtil.Logger;
            }

            /// <summary>
            /// Gets the implementation instance of <see cref="IWebSocketService"/>
            /// </summary>
            public sealed override IWebSocketService GetWebSocketService { get; }

            /// <summary>
            /// Gets the implementation instance of <see cref="IPropertiesService"/>
            /// </summary>
            public sealed override IPropertiesService GetPropertiesService { get; }

            /// <summary>
            /// Gets the implementation instance of <see cref="INtpSyncService"/>
            /// </summary>
            public sealed override INtpSyncService GetNtpSyncService { get; }

            /// <summary>
            /// Gets the implementation instance of <see cref="ISubjectViewService"/>
            /// </summary>
            public sealed override ISubjectViewService GetSubjectViewService { get; }

            /// <summary>
            /// Gets the implementation instance of <see cref="IBandManagerService"/>
            /// </summary>
            public sealed override IBandManagerService GetBandManagerService { get; }

            /// <summary>
            /// Gets the implementation of <see cref="ILogger"/>
            /// </summary>
            public sealed override ILogger GetLogger { get; }
        }

        private static readonly Lazy<ServiceFactory> serviceFactoryInstance = new Lazy<ServiceFactory>(() => new ServiceFactoryImpl());

        /// <summary>
        /// Gets the singleton instance of <see cref="ServiceFactory"/>
        /// </summary>
        public static ServiceFactory Singleton => serviceFactoryInstance.Value;

        /// <summary>
        /// Gets the implementation instance of <see cref="IWebSocketService"/>
        /// </summary>
        public abstract IWebSocketService GetWebSocketService { get; }

        /// <summary>
        /// Gets the implementation instance of <see cref="IPropertiesService"/>
        /// </summary>
        public abstract IPropertiesService GetPropertiesService { get; }

        /// <summary>
        /// Gets the implementation instance of <see cref="INtpSyncService"/>
        /// </summary>
        public abstract INtpSyncService GetNtpSyncService { get; }

        /// <summary>
        /// Gets the implementation instance of <see cref="ISubjectViewService"/>
        /// </summary>
        public abstract ISubjectViewService GetSubjectViewService { get; }

        /// <summary>
        /// Gets the implementation instance of <see cref="IBandManagerService"/>
        /// </summary>
        public abstract IBandManagerService GetBandManagerService { get; }

        /// <summary>
        /// Gets the implementation of <see cref="ILogger"/>
        /// </summary>
        public abstract ILogger GetLogger { get; }
    }
}
