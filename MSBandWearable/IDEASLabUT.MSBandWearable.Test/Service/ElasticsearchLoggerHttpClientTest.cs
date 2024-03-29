﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
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
using static System.Reflection.BindingFlags;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Unit test for <see cref="ElasticsearchLoggerHttpClient"/>
    /// </summary>
    [TestClass]
    public class ElasticsearchLoggerHttpClientTest : BaseHyperMock<ElasticsearchLoggerHttpClient>
    {
        [TestMethod]
        public async Task ShouldPostAsync()
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Setup(restClient => restClient.BulkRequestAsync("https://fake-url", stream)).Returns(Task.FromResult(response)));
            
            var actualResponse = await Subject.PostAsync("https://fake-url", stream);

            Assert.AreEqual(response, actualResponse, "Response should match");
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.BulkRequestAsync("https://fake-url", stream), Once()));
        }

        [TestMethod]
        public void ShouldNotConfigureForNullConfiguration()
        {
            Subject.Configure(null);
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.SetDefaultAuthenticationHeader(Param.IsAny<AuthenticationHeaderValue>()), Never()));
        }

        [TestMethod]
        public void ShouldNotConfigureForEmptySection()
        {
            MockFor<IConfiguration>(configurationMock => configurationMock.Setup(configuration => configuration.GetSection(Param.IsAny<string>())).Returns(MockValue<IConfigurationSection>()));
            Subject.Configure(MockValue<IConfiguration>());
            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.SetDefaultAuthenticationHeader(Param.IsAny<AuthenticationHeaderValue>()), Never()));
        }

        [TestMethod]
        public void ShouldConfigure()
        {
            MockFor<IConfigurationSection>(sectionMock => sectionMock.SetupGet(section => section.Value).Returns("Fake-Password"));
            MockFor<IConfiguration>(configurationMock => configurationMock.Setup(configuration => configuration.GetSection("elasticsearch:authenticationKey")).Returns(MockValue<IConfigurationSection>()));

            Subject.Configure(MockValue<IConfiguration>());
            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", "Fake-Password");

            MockFor<IElasticsearchRestClient>(restClientMock => restClientMock.Verify(restClient => restClient.SetDefaultAuthenticationHeader(authenticationHeaderValue), Once()));
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
            Assert.IsTrue(IsDisposed(), "Subject is disposed");
        }
    }
}
