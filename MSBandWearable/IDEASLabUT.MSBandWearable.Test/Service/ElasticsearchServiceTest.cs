using HyperMock;

using IDEASLabUT.MSBandWearable.Test;

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using static HyperMock.Occurred;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Unit test for <see cref="ElasticsearchService"/>
    /// </summary>
    [TestClass]
    public class ElasticsearchServiceTest : BaseHyperMock<ElasticsearchService>
    {
        [TestMethod]
        public async Task ShouldPostAsync()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            MockFor<IConfigurationSection>(sectionMock => sectionMock.SetupGet(section => section.Value).Returns("Fake-Password"));
            MockFor<IConfiguration>(configurationMock => configurationMock.Setup(configuration => configuration.GetSection("elasticsearch:authenticationKey")).Returns(MockValue<IConfigurationSection>()));
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Setup(restClient => restClient.BulkRequestAsync("https://fake-url", "{}", Param.IsAny<AuthenticationHeaderValue>())).Returns(Task.FromResult(response)));
            
            var actualResponse = await Subject.PostAsync("https://fake-url", stream);

            Assert.AreEqual(response, actualResponse);
            MockFor<IConfiguration>(configurationMock => configurationMock.Verify(configuration => configuration.GetSection("elasticsearch:authenticationKey"), Once()));
            MockFor<IConfigurationSection>(sectionMock => sectionMock.VerifyGet(section => section.Value, Once()));
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.BulkRequestAsync("https://fake-url", "{}", Param.IsAny<AuthenticationHeaderValue>()), Once()));
        }
        
        [TestMethod]
        public void ConfigureShouldDoNothing()
        {
            Subject.Configure(null);
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.Dispose(), Never()));
            MockFor<IConfiguration>(configurationMock => configurationMock.Verify(configuration => configuration.GetSection(Param.IsAny<string>()), Never()));
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.BulkRequestAsync(Param.IsAny<string>(), Param.IsAny<string>(), Param.IsAny<AuthenticationHeaderValue>()), Never()));
        }

        [TestMethod]
        public void Dispose()
        {
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Setup(restClient => restClient.Dispose()));
            Subject.Dispose();
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.Dispose(), Once()));
        }
    }
}
