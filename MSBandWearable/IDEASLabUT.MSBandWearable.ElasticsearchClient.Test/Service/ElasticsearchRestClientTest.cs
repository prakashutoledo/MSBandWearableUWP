
using HyperMock;

using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichardSzalay.MockHttp;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using static HyperMock.Occurred;
using static System.Net.Http.HttpMethod;
using static System.Net.HttpStatusCode;
using static System.Reflection.BindingFlags;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A unit test for <see cref="ElasticsearchRestClient"/>
    /// </summary>
    [TestClass]
    public class ElasticsearchRestClientTest : BaseHyperMock<ElasticsearchRestClient>
    {
        private HttpClient httpClient;

        public override void OverrideMockSetup()
        {
            MockFor<IHttpClientSupplier>(supplierMock =>
            {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp.Expect(Post, "https://some-fake-elasticsearch-base-url/_bulk")
                    .WithHeaders("Content-Type", "application/json")
                    .WithContent("{}")
                    .Respond("application/json", "{}");
                httpClient = mockHttp.ToHttpClient();
                _ = supplierMock.Setup(supplier => supplier.Supply()).Returns(httpClient);
            });
        }

        [TestMethod]
        public void ShouldOverrideDefaultAuthenticationHeaderValue()
        {
            var authenticationHeader = new AuthenticationHeaderValue("Basic", "abcd");
            Subject.SetDefaultAuthenticationHeader(authenticationHeader);

            var actualAuthenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            Assert.IsNotNull(actualAuthenticationHeader, "Authorization header should not be null");
            Assert.AreEqual(authenticationHeader, actualAuthenticationHeader, "Authentication headers should match");
        }

        [TestMethod]
        public async Task ShouldBulkRequestAsync()
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            var response = await Subject.BulkRequestAsync("https://some-fake-elasticsearch-base-url", memoryStream);

            MockFor<IHttpClientSupplier>(supplierMock => supplierMock.Verify(supplier => supplier.Supply(), Exactly(1)));
            Assert.IsNotNull(response, "Response shouldn't be null");

            var actualContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("{}", actualContent, "Response content should match");
            Assert.AreEqual(OK, response.StatusCode, "Request should create a resource");
        }

        [TestMethod]
        public void ShouldDispose()
        {
            var disposedValue = Subject.GetType().GetField("disposedValue", Instance | NonPublic);
            bool IsDisposed() => (bool) disposedValue.GetValue(Subject);
            Assert.IsFalse(IsDisposed(), "Subject is not disposed");

            Subject.Dispose();
            Assert.IsTrue(IsDisposed(), "Subject has been disposed");
        }

        [TestMethod]
        public async Task ShouldThrowExceptionWhileBulkRequest()
        {
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => Subject.BulkRequestAsync(" ", null));
            Assert.AreEqual("baseElasticsearchURI", exception.ParamName, "Base elasticsearch URI is null");

            exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => Subject.BulkRequestAsync("", null));
            Assert.AreEqual("baseElasticsearchURI", exception.ParamName, "Base elasticsearch URI is null");

            exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => Subject.BulkRequestAsync("{}", null));
            Assert.AreEqual("bulkRequestBody", exception.ParamName, "Request body is null");
        }
    }
}
