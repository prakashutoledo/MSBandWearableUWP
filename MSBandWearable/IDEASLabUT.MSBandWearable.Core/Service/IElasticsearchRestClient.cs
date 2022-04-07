using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Core.Service
{
    /// <summary>
    /// An interface for providing Elasticsearch rest client
    /// </summary>
    public interface IElasticsearchRestClient
    {
        /// <summary>
        /// Performs bulk POST request for given bulk request body with given authentication header to given base elasticsearch url
        /// </summary>
        /// <param name="baseElasticsearchURI">A base elasticsearch uri to perform bulk request</param>
        /// <param name="bulkRequestBody">A bulk POST request body</param>
        /// <param name="authenticationHeaderValue">A header value with authentication details</param>
        /// <returns>A http response message task that can be awaited</returns>
        Task<HttpResponseMessage> BulkRequestAsync(string baseElasticsearchURI, string bulkRequestBody, AuthenticationHeaderValue authenticationHeaderValue);

        /// <summary>
        /// Dispose the underlying http client
        /// </summary>
        void Dispose();
    }
}
