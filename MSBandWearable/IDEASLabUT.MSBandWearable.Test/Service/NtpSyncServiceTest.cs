using GuerrillaNtp;

using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Service
{
    [TestClass]
    public class NtpSyncServiceTest : BaseHyperMock<NtpSyncService>
    {
        [TestMethod]
        public async Task ShouldSyncTimestamp()
        {
            var ipaddress = await Dns.GetHostAddressesAsync("pool.ntp.org");
            MockFor<INtpClientSupplier>(clientMock => clientMock.Setup(client => client.Supply("pool.ntp.org")).Returns(Task.FromResult(new NtpClient(ipaddress.First()))));
            await Subject.SyncTimestamp("pool.ntp.org");
            Assert.IsTrue(Subject.Synced, "Time is synced");
        }

        [TestMethod]
        public async Task ShouldFailedToSyncTimestamp()
        {
            MockFor<INtpClientSupplier>(clientMock => clientMock.Setup(client => client.Supply("pool.ntp.or")).Returns(Task.FromResult((NtpClient) null)));
            await Subject.SyncTimestamp("pool.ntp.or");
            Assert.IsFalse(Subject.Synced, "Failed to sync timestamp");
            
            MockFor<INtpClientSupplier>(clientMock => clientMock.Setup(client => client.Supply("pool.ntp.or")).Returns(Task.FromException<NtpClient>(new SocketException())));
            await Subject.SyncTimestamp("pool.ntp.or");
            Assert.IsFalse(Subject.Synced, "Failed to sync timestamp");
        }
    }
}
