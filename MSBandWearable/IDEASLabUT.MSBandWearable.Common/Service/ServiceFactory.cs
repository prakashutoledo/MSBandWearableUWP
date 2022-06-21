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
                GetBandClientService = MSBandClientService.Singleton;
                GetPropertiesService = PropertiesService.Singleton;
            }

            /// <inheritdoc/>
            public sealed override IWebSocketService GetWebSocketService { get; }

            /// <inheritdoc/>
            public sealed override IBandClientService GetBandClientService { get; }

            /// <inheritdoc/>
            public sealed override IPropertiesService GetPropertiesService { get; }
        }

        private static readonly Lazy<ServiceFactory> serviceFactoryInstance;

        static ServiceFactory()
        {
            serviceFactoryInstance = new Lazy<ServiceFactory>(() => new ServiceFactoryImpl());
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="ServiceFactory"/>
        /// </summary>
        public static ServiceFactory Singleton => serviceFactoryInstance.Value;

        /// <summary>
        /// Gets the implementation instance of <see cref="IWebSocketService"/>
        /// </summary>
        public abstract IWebSocketService GetWebSocketService { get; }

        /// <summary>
        /// Gets the implementation instance of <see cref="IBandClientService"/>
        /// </summary>
        public abstract IBandClientService GetBandClientService { get; }

        /// <summary>
        /// Gets the implementation instance of <see cref="IPropertiesService"/>
        /// </summary>
        public abstract IPropertiesService GetPropertiesService { get; }
    }
}
