using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using static System.Net.Http.HttpMethod;
using static System.Text.Encoding;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Elasticsearch rest client for providing bulk request api using <see cref="HttpClient"/>
    /// </summary>
    public class ElasticsearchRestClient : IElasticsearchRestClient
    {
        private static readonly Lazy<ElasticsearchRestClient> Instance = new Lazy<ElasticsearchRestClient>(() => new ElasticsearchRestClient(new HttpClient()));

        // Lazy singleton pattern
        public static ElasticsearchRestClient Singleton => Instance.Value;

        private const string JsonContentType = "application/json";

        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of <see cref="ElasticsearchRestClient"/>
        /// </summary>
        /// <param name="httpClient">A http client to set</param>
        /// <exception cref="ArgumentNullException">If httpClient is null</exception>
        private ElasticsearchRestClient(HttpClient httpClient)
        {
            // private instantiation
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Performs bulk POST request for given bulk request body with given authentication header to given base elasticsearch url
        /// </summary>
        /// <param name="baseElasticsearchURI">A base elasticsearch uri to perform bulk request</param>
        /// <param name="bulkRequestBody">A bulk POST request body</param>
        /// <param name="authenticationHeaderValue">A header value with authentication details</param>
        /// <returns>A http response message task that can be awaited</returns>
        /// <exception cref="ArgumentNullException">If baseElasticsearchURI, requestBody or authenticationHeaderValue is null or empty</exception>
        public async Task<HttpResponseMessage> BulkRequestAsync(string baseElasticsearchURI, string requestBody, AuthenticationHeaderValue authenticationHeaderValue)
        {
            if (string.IsNullOrEmpty(baseElasticsearchURI))
            {
                throw new ArgumentNullException(nameof(baseElasticsearchURI));
            }

            if (string.IsNullOrEmpty(requestBody))
            {
                throw new ArgumentNullException(nameof(requestBody));
            }

            if (authenticationHeaderValue == null)
            {
                throw new ArgumentNullException(nameof(authenticationHeaderValue));
            }

            HttpResponseMessage response;
            var elasticsearchURI = $"{baseElasticsearchURI}/_bulk";
            using (var bulkPostRequest = new HttpRequestMessage(Post, elasticsearchURI))
            using (var bulkRequestContent = new StringContent(requestBody, UTF8, JsonContentType))
            {
                bulkRequestContent.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);
                bulkPostRequest.Headers.Authorization = authenticationHeaderValue;
                bulkPostRequest.Content = bulkRequestContent;
                response = await httpClient.SendAsync(bulkPostRequest);
            }
            return response;
        }

        /// <summary>
        /// Dispose the underlying <see cref="HttpClient"/>
        /// </summary>
        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}