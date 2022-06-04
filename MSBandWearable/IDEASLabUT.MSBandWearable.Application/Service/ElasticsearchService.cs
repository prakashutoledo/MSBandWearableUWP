﻿using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;
using static IDEASLabUT.MSBandWearable.Application.MSBandWearableApplicationGlobals;

using IDEASLabUT.MSBandWearable.Core.Service;

using System;
using System.IO;
using System.Net.Http;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;
using System.Net.Http.Headers;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    /// <summary>
    /// An Elasticserach service which performs bulk api request using <see cref="IElasticsearchRestClient"/>
    /// </summary>
    public class ElasticsearchService : IHttpClient
    {
        private static readonly Lazy<ElasticsearchService> Instance = new Lazy<ElasticsearchService>(() => new ElasticsearchService(ApplicationProperties, ElasticsearchRestClient.Singleton));

        // Lazy singleton pattern
        public static ElasticsearchService Singleton => Instance.Value;

        private const string BasicAuthorization = "Basic";

        private readonly IElasticsearchRestClient elasticsearchRestClient;
        private readonly IConfiguration applicationProperties;

        /// <summary>
        /// Initializes a new instance of <see cref="ElasticsearchService"/>
        /// </summary>
        /// <param name="applicationProperties">An application properties to set</param>
        /// <param name="elasticsearchRestClient">An elasticsearch rest client to set</param>
        /// <exception cref="ArgumentNullException">If any of the parameters applicationProperties or elasticsearchRestClient is null</exception>
        private ElasticsearchService(IConfiguration applicationProperties, IElasticsearchRestClient elasticsearchRestClient)
        {
            // private initialization
            this.applicationProperties = applicationProperties ?? throw new ArgumentNullException(nameof(applicationProperties));
            this.elasticsearchRestClient = elasticsearchRestClient ?? throw new ArgumentNullException(nameof(elasticsearchRestClient));
        }

        /// <inheritdoc />
        public virtual void Configure(IConfiguration configuration)
        {
            // not needed at this point
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
        {
            var jsonBody = await new StreamReader(contentStream).ReadToEndAsync();
            var authenticationHeaderValue = new AuthenticationHeaderValue(BasicAuthorization, applicationProperties.GetValue<string>(ElasticsearchAuthenticationJsonKey));
            var response = await elasticsearchRestClient.BulkRequestAsync(requestUri, jsonBody, authenticationHeaderValue);
            return response;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the inclosed elasticsearch rest client if given paramater is set to true otherwise ignored
        /// </summary>
        /// <param name="disposing">a boolean parameter for disposing elasticsearch rest client</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                elasticsearchRestClient.Dispose();
            }
        }

        /// <summary>
        /// Destructor for elasticsearch rest service instance
        /// </summary>
        ~ElasticsearchService()
        {
            Dispose(false);
        }
    }
}
