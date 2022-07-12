using IDEASLabUT.MSBandWearable.Test;

using Microsoft.Band;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Unit test for <see cref="MSBandClientService"/>
    /// </summary>
    [TestClass]
    public class MSBandClientServiceTest : BaseHyperMock<MSBandClientService>
    {
        [DataTestMethod]
        [DataRow(null, DisplayName = "Null bandName")]
        [DataRow("", DisplayName = "Empty(\"\") bandName")]
        [DataRow(" ", DisplayName = "Empty(\" \") bandName")]
        [DataRow("   ", DisplayName = "Empty(\"   \")  bandName")]
        public async Task ShouldNotConnectForNullOrEmptyBandName(string emptyBandName)
        {
            var actualException = await Assert.ThrowsExceptionAsync<ArgumentException>(() => Subject.ConnectBand(emptyBandName));
            Assert.AreEqual("Invalid bandName empty or null", actualException.Message);
        }

        [DataTestMethod]
        [DynamicData(nameof(NullOrEmptyPairedBandsSupplier),  DynamicDataSourceType.Method)]
        public async Task ShouldNotConnectForNullOrEmptyPairedBands(IBandInfo[] pairedBands)
        {
            MockFor<IBandClientManager>(clientBandManagerMock => clientBandManagerMock.Setup(client => client.GetBandsAsync(true)).Returns(Task.FromResult(pairedBands)));
            var actualException = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Subject.ConnectBand("Fake Band Name"));
            Assert.AreEqual("No paired bands available", actualException.Message, "No paired bands available");
        }

        private static IEnumerable<object[]> NullOrEmptyPairedBandsSupplier()
        {
            return new List<object[]>()
            {
                new object[] { null },
                new object[] { new IBandInfo[] { } },
            };
        }
    }
}
