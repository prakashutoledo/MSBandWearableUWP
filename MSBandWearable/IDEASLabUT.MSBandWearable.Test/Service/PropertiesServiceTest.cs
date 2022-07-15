/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using static HyperMock.Occurred;
using static HyperMock.Param;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Unit test for <see cref="PropertiesService"/>
    /// </summary>
    [TestClass]
    public class PropertiesServiceTest : BaseHyperMock<PropertiesService>
    {
        [TestMethod]
        public void ShouldGetProperties()
        {
            Assert.AreEqual(MockValue<IConfiguration>(), Subject.GetProperties, "Should return mocked configuration");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("     ")]
        public void ShouldThrowExceptionForInvalidPropertyKey(string invalidKey)
        {
            var exceptionKey = Assert.ThrowsException<ArgumentNullException>(() => Subject.GetProperty(invalidKey), "Not a valid property key");
            Assert.AreEqual("key", exceptionKey.ParamName, "");
            MockFor<IConfiguration>(configurationMock => configurationMock.Verify(configuration => configuration.GetSection(IsAny<string>()), Never()));
        }

        [DataTestMethod]
        [DataRow("key1")]
        [DataRow("key2")]
        [DataRow("key3")]
        public void ShouldGetNullValueForPropertySectionThatDoesntExist(string key)
        {
            Assert.IsNull(Subject.GetProperty(key), $"Property section is null for {key}");
            MockFor<IConfiguration>(configurationMock => configurationMock.Verify(configuration => configuration.GetSection(key), Once()));
        }

        [DataTestMethod]
        [DataRow("key4")]
        [DataRow("key5")]
        [DataRow("key6")]
        public void ShouldGetNullValueForPropertyKeyThatDoesntExist(string key)
        {
            MockFor<IConfiguration>(configurationMock => configurationMock.Setup(configuration => configuration.GetSection(IsAny<string>())).Returns(MockValue<IConfigurationSection>()));
            Assert.IsNull(Subject.GetProperty(key), $"Property section value is null for {key}");
            MockFor<IConfiguration>(configurationMock => configurationMock.Verify(configuration => configuration.GetSection(key), Once()));
            MockFor<IConfigurationSection>(sectionMock => sectionMock.VerifyGet(section => section.Value, Once()));
        }

        [TestMethod]
        public void ShouldGetProperty()
        {
            MockFor<IConfiguration>(configurationMock => configurationMock.Setup(configuration => configuration.GetSection(IsAny<string>())).Returns(MockValue<IConfigurationSection>()));
            MockFor<IConfigurationSection>(sectionMock => sectionMock.SetupGet(section => section.Value).Returns("fake-value"));
            Assert.AreEqual("fake-value", Subject.GetProperty("any-key"), "Should get property value for given key");
            MockFor<IConfiguration>(configurationMock => configurationMock.Verify(configuration => configuration.GetSection("any-key"), Once()));
        }
    }
}
