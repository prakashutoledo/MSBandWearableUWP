using static IDEASLabUT.MSBandWearable.Model.BandStatus;
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;
using Microsoft.Band.Sensors;

namespace IDEASLabUT.MSBandWearable.Service
{
    [TestClass]
    public class MSBandManagerServiceTest : BaseHyperMock<MSBandManagerService>
    {
        [TestMethod]
        public async Task ShouldConnectBand()
        {
            Assert.AreEqual(UNKNOWN, Subject.BandStatus, "Before connection status is unknown");
            MockFor<IBandClientService>(clientMock => clientMock.Setup(client => client.ConnectBand("Fake-Band-Name")).Returns(Task.CompletedTask));

            await Subject.ConnectBand("Fake-Band-Name");
            
            Assert.AreEqual("Fake-Band-Name", Subject.BandName, "Band name should match");
            Assert.AreEqual(Connected, Subject.BandStatus, "Band status is connected");
        }

        [DataTestMethod]
        [DynamicData(nameof(FailedTaskSupplier), DynamicDataSourceType.Method)]
        public async Task ShouldNotConnectBandForFailedConnection(Task failedTask)
        {
            Assert.AreEqual(UNKNOWN, Subject.BandStatus, "Before connection status is unknown");
            Assert.IsNull(Subject.BandName, "No connected band name before connection");

            MockFor<IBandClientService>(clientMock => clientMock.Setup(client => client.ConnectBand("Band-Name")).Returns(failedTask));

            await Subject.ConnectBand("Band-Name");

            Assert.IsNull(Subject.BandName, "No connected band name after connection failed");
            Assert.AreEqual(Error, Subject.BandStatus, "After connection status is error");
        }

        public static IEnumerable<object[]> FailedTaskSupplier()
        {
            return new List<object[]>
            {
                new object[] { Task.FromException(new Exception()) },
                new object[] { Task.FromCanceled(new CancellationToken(true)) }
            };
        }
    }
}
