using GuerrillaNtp;
using System;
using System.Net;
using System.Threading;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class NtpSyncService
    {
        private static readonly Lazy<NtpSyncService> Instance = new Lazy<NtpSyncService>(() => new NtpSyncService());

        // Lazy singleton pattern
        public static NtpSyncService Singleton => Instance.Value;

        private NtpSyncService()
        {
            // private initialization
        }

        private ThreadLocal<TimeSpan> Offset { get; } = new ThreadLocal<TimeSpan>(() => TimeSpan.Zero);

        public DateTime LocalTimeNow => DateTime.Now + Offset.Value;

        public void SyncTimestamp(string url = "pool.ntp.org")
        {
            using (NtpClient ntp = new NtpClient(Dns.GetHostAddresses(url)[0]))
            {
                Offset.Value = ntp.GetCorrectionOffset();
            }
        }
    }
}
