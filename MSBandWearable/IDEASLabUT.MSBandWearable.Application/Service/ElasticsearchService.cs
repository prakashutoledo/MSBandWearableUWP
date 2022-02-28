using static IDEASLabUT.MSBandWearable.Application.MSBandWearableApplicationGlobals;

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class ElasticsearchService : IHttpClient
    {
        private const string BasicAuthorization = "Basic";

        private readonly IElasticsearchRestClient elasticsearchRestClient;
        private readonly string elastisearchAuthenticationKey;

        public ElasticsearchService(IConfiguration applicationProperties) : this(applicationProperties, ElasticsearchRestClient.Singleton)
        {
        }

        public ElasticsearchService(IConfiguration applicationProperties, IElasticsearchRestClient elasticsearchRestClient)
        {
            elastisearchAuthenticationKey = applicationProperties.GetValue<string>(ElasticsearchAuthenticationJsonKey);
            this.elasticsearchRestClient = elasticsearchRestClient;
        }

        public virtual void Configure(IConfiguration configuration)
        {
            // not needed at this point
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
        {
            string jsonBody = await new StreamReader(contentStream).ReadToEndAsync();
            HttpResponseMessage response = await elasticsearchRestClient.BulkRequestAsync(requestUri, jsonBody, new AuthenticationHeaderValue(BasicAuthorization, elastisearchAuthenticationKey));
            Trace.WriteLine(await response.Content.ReadAsStringAsync()); 
            return response;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                elasticsearchRestClient.Dispose();
            }
        }

        ~ElasticsearchService()
        {
            Dispose(false);
        }
    }
}
