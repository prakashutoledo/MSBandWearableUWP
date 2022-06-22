using Microsoft.Extensions.Configuration;

using Serilog.Sinks.Http;

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using static IDEASLabUT.MSBandWearable.ElasticsearchLoggerGlobals;

namespace IDEASLabUT.MSBandWearable.Service
{
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

        private ElasticsearchLoggerHttpClient(IElasticsearchRestClient elasticsearchRestClient)
        {
            this.elasticsearchRestClient = elasticsearchRestClient ?? throw new ArgumentNullException(nameof(elasticsearchRestClient));
        }

        public void Configure(IConfiguration configuration)
        {
            Trace.WriteLine(configuration.GetSection(ElasticsearchAuthenticationJsonKey).Value);
            elasticsearchRestClient.SetDefaultAuthenticationHeader(
                new AuthenticationHeaderValue(
                    BasicAuthorization,
                    configuration.GetSection(ElasticsearchAuthenticationJsonKey).Value
                )
            );
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
        {
            return await elasticsearchRestClient.BulkRequestAsync(requestUri, contentStream);
        }

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

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~ElasticsearchLoggerHttpClient()
        {
            Dispose(false);
        }
    }
}
