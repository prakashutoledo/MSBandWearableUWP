using static System.Net.Http.HttpMethod;
using static System.Text.Encoding;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public ElasticsearchService()
        {
            httpClient = new HttpClient();
        }

        ~ElasticsearchService()
        {
            Dispose(false);
        }

        public virtual void Configure(IConfiguration configuration)
        {
        }
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            HttpResponseMessage response;
            string json = await content.ReadAsStringAsync().ConfigureAwait(false);

            using (HttpRequestMessage postRequest= new HttpRequestMessage(Post, requestUri))
            using (StringContent stringContent = new StringContent(json, UTF8, JsonContentType))
            {
                stringContent.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);
                postRequest.Headers.Authorization = new AuthenticationHeaderValue(BasicAuthorization, System.Convert.ToBase64String(UTF8.GetBytes("ideaslabut:9845315216@Pk")));
                postRequest.Content = content;
                response = await httpClient.SendAsync(postRequest).ConfigureAwait(false);
            }

            System.Diagnostics.Trace.WriteLine(response.StatusCode);
            System.Diagnostics.Trace.WriteLine(await response.Content.ReadAsStringAsync());
            System.Diagnostics.Trace.WriteLine("-------------------------------------------------------------------------------------------------");
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
    }
}
