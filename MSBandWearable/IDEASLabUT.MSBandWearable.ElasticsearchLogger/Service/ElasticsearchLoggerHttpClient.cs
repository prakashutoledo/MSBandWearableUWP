using Microsoft.Extensions.Configuration;

using Serilog.Sinks.Http;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using static IDEASLabUT.MSBandWearable.ElasticsearchLoggerGlobals;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A serilog <see cref="IHttpClient"/> implementation for logging data into elasticsearch
    /// </summary>
    public sealed class ElasticsearchLoggerHttpClient : IHttpClient
    {
        private const string BasicAuthorization = "Basic";
        private static readonly Lazy<IHttpClient> ElasticsearchLoggerHttpClientInstance;

        static ElasticsearchLoggerHttpClient()
        {
            ElasticsearchLoggerHttpClientInstance = new Lazy<IHttpClient>(() => new ElasticsearchLoggerHttpClient(ElasticsearchRestClient.Singleton));
        }

        internal static IHttpClient Singleton => ElasticsearchLoggerHttpClientInstance.Value;

        private bool disposedValue;
        private readonly IElasticsearchRestClient elasticsearchRestClient;

        /// <summary>
        /// Creates a new instance of <see cref="ElasticsearchLoggerHttpClient"/>
        /// </summary>
        /// <param name="elasticsearchRestClient">An elasticsearch rest client to set</param>
        /// <exception cref="ArgumentNullException">If elasticsearch rest client is null</exception>
        private ElasticsearchLoggerHttpClient(IElasticsearchRestClient elasticsearchRestClient)
        {
            this.elasticsearchRestClient = elasticsearchRestClient ?? throw new ArgumentNullException(nameof(elasticsearchRestClient));
        }

        /// <inheritdoc/>
        public void Configure(IConfiguration configuration)
        {
            elasticsearchRestClient.SetDefaultAuthenticationHeader(
                new AuthenticationHeaderValue(
                    BasicAuthorization,
                    configuration.GetSection(ElasticsearchAuthenticationJsonKey).Value
                )
            );
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
        {
            return await elasticsearchRestClient.BulkRequestAsync(requestUri, contentStream);
        }

        /// <summary>
        /// Dispose the underlying <see cref="IElasticsearchRestClient"/> based on given disposing flag
        /// </summary>
        /// <param name="disposing">A disposing flag to check if rest client needs to be disposed or not</param>
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    elasticsearchRestClient.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the underlying <see cref="IElasticsearchRestClient"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer for <see cref="ElasticsearchLoggerHttpClient"/>
        /// </summary>
        ~ElasticsearchLoggerHttpClient()
        {
            Dispose(false);
        }
    }
}
