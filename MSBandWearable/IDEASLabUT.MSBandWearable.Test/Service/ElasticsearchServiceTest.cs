using HyperMock;

using IDEASLabUT.MSBandWearable.Test;

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using static HyperMock.Occurred;
using static System.Reflection.BindingFlags;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Unit test for <see cref="ElasticsearchLoggerHttpClient"/>
    /// </summary>
    [TestClass]
    public class ElasticsearchServiceTest : BaseHyperMock<ElasticsearchLoggerHttpClient>
    {
        [TestMethod]
        public async Task ShouldPostAsync()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            //MockFor<IConfigurationSection>(sectionMock => sectionMock.SetupGet(section => section.Value).Returns("Fake-Password"));
            //MockFor<IConfiguration>(configurationMock => configurationMock.Setup(configuration => configuration.GetSection("elasticsearch:authenticationKey")).Returns(MockValue<IConfigurationSection>()));
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Setup(restClient => restClient.BulkRequestAsync("https://fake-url", stream)).Returns(Task.FromResult(response)));
            
            var actualResponse = await Subject.PostAsync("https://fake-url", stream);

            Assert.AreEqual(response, actualResponse, "Response should match");
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.BulkRequestAsync("https://fake-url", stream), Once()));
        }
        
        [TestMethod]
        public void ShouldConfigure()
        {
            MockFor<IConfigurationSection>(sectionMock => sectionMock.SetupGet(section => section.Value).Returns("Fake-Password"));
            MockFor<IConfiguration>(configurationMock => configurationMock.Setup(configuration => configuration.GetSection("elasticsearch:authenticationKey")).Returns(MockValue<IConfigurationSection>()));

            Subject.Configure(MockValue<IConfiguration>());

            MockFor<IConfiguration>(configurationMock => configurationMock.Verify(configuration => configuration.GetSection("elasticsearch:authenticationKey"), Once()));
            MockFor<IConfigurationSection>(sectionMock => sectionMock.VerifyGet(section => section.Value, Once()));
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.Dispose(), Never()));
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.BulkRequestAsync(Param.IsAny<string>(), Param.IsAny<Stream>()), Never()));
        }

        [TestMethod]
        public void Dispose()
        {
            var disposedValue = Subject.GetType().GetField("disposedValue", Instance | NonPublic);
            bool IsDisposed() => (bool) disposedValue.GetValue(Subject);

            Assert.IsFalse(IsDisposed(), "Subject is not disposed");
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Setup(restClient => restClient.Dispose()));

            Subject.Dispose();
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.Dispose(), Once()));
            Assert.IsTrue(IsDisposed(), "Subject is disposed");
        }
    }
}
