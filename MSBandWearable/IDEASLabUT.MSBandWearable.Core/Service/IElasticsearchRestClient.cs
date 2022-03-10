﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Core.Service
{
    public interface IElasticsearchRestClient
    {
        Task<HttpResponseMessage> BulkRequestAsync(string elasticsearchURI, string bulkRequestBody, AuthenticationHeaderValue authenticationHeaderValue);
        void Dispose();
    }
}