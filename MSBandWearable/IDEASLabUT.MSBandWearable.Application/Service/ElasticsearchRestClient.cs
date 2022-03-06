using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class ElasticsearchRestClient : IElasticsearchRestClient
    {
        private static readonly Lazy<ElasticsearchRestClient> Instance = new Lazy<ElasticsearchRestClient>(() => new ElasticsearchRestClient(new HttpClient()));

        // Lazy singleton pattern
        public static ElasticsearchRestClient Singleton => Instance.Value;

        private const string JsonContentType = "application/json";

        private readonly HttpClient httpClient;

        public ElasticsearchRestClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> BulkRequestAsync(string elasticsearchURI, string requestBody, AuthenticationHeaderValue authenticationHeaderValue)
        {
            HttpResponseMessage response;
            using (var bulkPostRequest = new HttpRequestMessage(HttpMethod.Post, elasticsearchURI))
            using (var bulkRequestContent = new StringContent(requestBody, Encoding.UTF8, JsonContentType))
            {
                bulkRequestContent.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);
                bulkPostRequest.Headers.Authorization = authenticationHeaderValue;
                bulkPostRequest.Content = bulkRequestContent;
                response = await httpClient.SendAsync(bulkPostRequest);
            }
            return response;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}