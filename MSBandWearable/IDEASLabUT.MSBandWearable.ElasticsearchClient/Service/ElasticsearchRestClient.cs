/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Elasticsearch rest client for providing bulk request api using <see cref="HttpClient"/>
    /// </summary>
    public sealed class ElasticsearchRestClient : IElasticsearchRestClient
    {
        private static readonly Lazy<ElasticsearchRestClient> ElasticsearchRestClientInstance;
        private static readonly MediaTypeHeaderValue MediaTypeJson;

        static ElasticsearchRestClient()
        {
            ElasticsearchRestClientInstance = new Lazy<ElasticsearchRestClient>(() => new ElasticsearchRestClient(new HttpClientSupplier()));
            MediaTypeJson = new MediaTypeHeaderValue("application/json");
        }

        // Lazy singleton pattern
        internal static ElasticsearchRestClient Singleton => ElasticsearchRestClientInstance.Value;

        private readonly HttpClient httpClient;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of <see cref="ElasticsearchRestClient"/>
        /// </summary>
        /// <param name="HttpClientSupplier">A http client supplier to set</param>
        /// <exception cref="ArgumentNullException">If httpClient is null</exception>
        private ElasticsearchRestClient(IHttpClientSupplier httpClientSupplier)
        {
            if (httpClientSupplier == null)
            {
                throw new ArgumentNullException(nameof(httpClientSupplier));
            }

            httpClient = httpClientSupplier.Supply();
        }

        /// <summary>
        /// Add default authentication header for every request to underlying http client
        /// </summary>
        /// <param name="authenticationHeaderValue">An authentication header value to set</param>
        public void SetDefaultAuthenticationHeader(AuthenticationHeaderValue authenticationHeaderValue)
        {
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        }

        /// <summary>
        /// Performs bulk POST request for given bulk request body with given authentication header to given base elasticsearch url
        /// </summary>
        /// <param name="baseElasticsearchURI">A base elasticsearch uri to perform bulk request</param>
        /// <param name="bulkRequestBody">A bulk POST request body stream</param>
        /// <returns>A http response message task that can be awaited</returns>
        /// <exception cref="ArgumentNullException">If baseElasticsearchURI, requestBody or authenticationHeaderValue is null or empty</exception>
        public Task<HttpResponseMessage> BulkRequestAsync(string baseElasticsearchURI, Stream bulkRequestBody)
        {
            if (string.IsNullOrWhiteSpace(baseElasticsearchURI))
            {
                throw new ArgumentNullException(nameof(baseElasticsearchURI));
            }

            if (bulkRequestBody == null)
            {
                throw new ArgumentNullException(nameof(bulkRequestBody));
            }

            var streamContent = new StreamContent(bulkRequestBody);
            streamContent.Headers.ContentType = MediaTypeJson;
            return httpClient.PostAsync($"{baseElasticsearchURI}/_bulk", streamContent);
        }

        /// <summary>
        /// Dispose the underlying <see cref="HttpClient"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the underlying <see cref="HttpClient"/> based on given disposing flag
        /// </summary>
        /// <param name="disposing">A disposing flag to check if HttpClient needs to be disposed or not</param>
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    httpClient.Dispose();
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer for <see cref="ElasticsearchRestClient"/>
        /// </summary>
        ~ElasticsearchRestClient()
        {
            Dispose(disposing: false);
        }
    }
}