using GuerrillaNtp;
using System;
using System.Net;
using System.Threading;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class NtpSyncService
    {
        private static readonly Lazy<NtpSyncService> Instance = new Lazy<NtpSyncService>(() => new NtpSyncService());
        private static object offset = TimeSpan.Zero;

        // Lazy singleton pattern
        public static NtpSyncService Singleton => Instance.Value;

        private NtpSyncService()
        {
            // private initialization
        }

        private TimeSpan Offset
        {
            get => (TimeSpan) offset;
            set => Interlocked.Exchange(ref offset, value);
        }

        /// <summary>
        /// The synchronized current timestamp with correction offset added
        /// </summary>
        public DateTime LocalTimeNow => DateTime.Now + Offset;

        /// <summary>
        /// Sunchronized the datetime for this application to given ntp pool by finding the datetime correction offset
        /// Default pool is 'pool.ntp.org'
        /// </summary>
        /// <param name="poolAddress">A ntp pool to get the correction offset</param>
        public void SyncTimestamp(string poolAddress = "pool.ntp.org")
        {
            // Only used the first address from the given pool
            using (NtpClient ntp = new NtpClient(Dns.GetHostAddresses(poolAddress)[0]))
            {
                Offset = ntp.GetCorrectionOffset();
            }
        }
    }
}
