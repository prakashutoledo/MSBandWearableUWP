/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// An interface for providing Elasticsearch rest client
    /// </summary>
    public interface IElasticsearchRestClient : IDisposable
    {
        /// <summary>
        /// Add default authentication header for every request to underlying http client
        /// </summary>
        /// <param name="authenticationHeaderValue">An authentication header value to set</param>
        void SetDefaultAuthenticationHeader(AuthenticationHeaderValue authenticationHeaderValue);

        /// <summary>
        /// Performs bulk POST request for given bulk request body with given authentication header to given base elasticsearch url
        /// </summary>
        /// <param name="baseElasticsearchURI">A base elasticsearch uri to perform bulk request</param>
        /// <param name="bulkRequestBody">A bulk POST request body</param>
        /// <returns>A http response message task that can be awaited</returns>
        Task<HttpResponseMessage> BulkRequestAsync(string baseElasticsearchURI, Stream bulkRequestBody);
    }
}
