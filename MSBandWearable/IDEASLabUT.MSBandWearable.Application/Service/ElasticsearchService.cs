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
        private const string JsonContentType = "application/json";
        private const string BasicAuthorization = "Basic";

        private readonly HttpClient httpClient;
        private readonly string elastisearchAuthenticationKey;

        public ElasticsearchService(IConfiguration applicationProperties) : this(applicationProperties, new HttpClient())
        {
        }

        public ElasticsearchService(IConfiguration applicationProperties, HttpClient httpClient)
        {
            elastisearchAuthenticationKey = applicationProperties.GetValue<string>(ElasticsearchAuthenticationJsonKey);
            this.httpClient = httpClient;
        }

        public virtual void Configure(IConfiguration configuration)
        {
            // not needed at this point
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
        {
            HttpResponseMessage response;
            string jsonBody = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);
            
            using (var postRequest = new HttpRequestMessage(HttpMethod.Post, requestUri))
            using (var postBody = new StringContent(jsonBody, Encoding.UTF8, JsonContentType))
            {
                postBody.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);
                postRequest.Headers.Authorization = new AuthenticationHeaderValue(BasicAuthorization, elastisearchAuthenticationKey);
                //postRequest.Headers.Authorization = new AuthenticationHeaderValue(BasicAuthorization, Convert.ToBase64String(Encoding.UTF8.GetBytes("ideaslabut:9845315216@Pk")));
                //postRequest.Headers.Authorization = new AuthenticationHeaderValue(BasicAuthorization, Convert.ToBase64String(Encoding.UTF8.GetBytes("ideaslabut:9845315216@Pk")));
                postRequest.Content = postBody;
                response = await httpClient.SendAsync(postRequest).ConfigureAwait(false);
            }

            Debug.WriteLine(response.StatusCode);
            Debug.WriteLine(await response.Content.ReadAsStringAsync()); 
            Debug.WriteLine("-------------------------------------------------------------------------------------------------");
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
                httpClient.Dispose();
            }
        }

        ~ElasticsearchService()
        {
            Dispose(false);
        }
    }
}
