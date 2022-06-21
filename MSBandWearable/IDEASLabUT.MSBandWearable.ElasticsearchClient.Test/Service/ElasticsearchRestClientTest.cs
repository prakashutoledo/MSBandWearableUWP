
using HyperMock;

using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichardSzalay.MockHttp;

using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using static HyperMock.Occurred;
using static System.Net.Http.HttpMethod;
using static System.Net.HttpStatusCode;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A unit test for <see cref="ElasticsearchRestClient"/>
    /// </summary>
    [TestClass]
    public class ElasticsearchRestClientTest : BaseHyperMock<ElasticsearchRestClient>
    {
        public override void OverrideMockSetup()
        {
            MockFor<IHttpClientSupplier>(supplierMock =>
            {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp.Expect(Post, "https://some-fake-elasticsearch-base-url/_bulk")
                    .WithHeaders("Content-Type", "application/json")
                    .WithHeaders("Authorization", "Basic abcde")
                    .WithContent("{}")
                    .Respond("application/json", "{}");
                _ = supplierMock.Setup(supplier => supplier.Supply()).Returns(mockHttp.ToHttpClient());
            });
        }

        [TestMethod]
        public async Task ShouldBulkRequestAsync()
        {
            var response = await Subject.BulkRequestAsync("https://some-fake-elasticsearch-base-url", "{}", new AuthenticationHeaderValue("Basic", "abcde"));
            MockFor<IHttpClientSupplier>(supplierMock => supplierMock.Verify(supplier => supplier.Supply(), Exactly(1)));
            Assert.IsNotNull(response, "Response shouldn't be null");
            var actualContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("{}", actualContent, "Response content should match");
            Assert.AreEqual(OK, response.StatusCode, "Request should create a resource");
        }

        [TestMethod]
        public async Task ShouldThrowExceptionWhileBulkRequest()
        {
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => Subject.BulkRequestAsync("", "{}", null));
            Assert.AreEqual("baseElasticsearchURI", exception.ParamName, "Base elasticsearch URI is null");

            exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => Subject.BulkRequestAsync("{}", "", null));
            Assert.AreEqual("requestBody", exception.ParamName, "Request body is null");

            exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => Subject.BulkRequestAsync("{}", "{}", null));
            Assert.AreEqual("authenticationHeaderValue", exception.ParamName, "Authentication header value is null");
        }
    }
}
