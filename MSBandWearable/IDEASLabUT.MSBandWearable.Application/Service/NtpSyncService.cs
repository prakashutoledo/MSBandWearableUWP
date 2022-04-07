using IDEASLabUT.MSBandWearable.Core.Service;

using GuerrillaNtp;

using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    /// <summary>
    /// A service class for performing timestamp synchronization using Guerrilla <see cref="NtpClient"/>
    /// </summary>
    public class NtpSyncService : INtpSyncService
    {
        private static readonly Lazy<NtpSyncService> Instance = new Lazy<NtpSyncService>(() => new NtpSyncService());
        private static object correctionOffset = TimeSpan.Zero;

        // Lazy singleton pattern
        public static NtpSyncService Singleton => Instance.Value;

        private NtpSyncService()
        {
            // private initialization
        }

        public TimeSpan CorrectionOffset
        {
            get => (TimeSpan) correctionOffset;
            set => Interlocked.Exchange(ref correctionOffset, value);
        }

        /// <summary>
        /// The synchronized current timestamp with correction offset added
        /// </summary>
        public DateTime LocalTimeNow => DateTime.Now + CorrectionOffset;

        /// <summary>
        /// Sunchronized the datetime for this application to given ntp pool by finding the datetime correction offset
        /// </summary>
        /// <param name="poolAddress">A ntp pool to get the correction offset</param>
        public void SyncTimestamp(string poolAddress)
        {
            // Only used the first address from the given pool
            using (var ntpClient = new NtpClient(Dns.GetHostAddresses(poolAddress)[0]))
            {
                CorrectionOffset = ntpClient.GetCorrectionOffset();
                Trace.WriteLine($"Succesfully synced to '{poolAddress}' with offset ({correctionOffset}).");
            }
        }
    }
}
