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

        /// <summary>
        /// Thread local storage for correction offset. Since, we are using this accross multiple threads it is always
        /// a good idea to make common constant value shared accross all available threads instead of making it static
        /// </summary>
        private ThreadLocal<TimeSpan> Offset { get; } = new ThreadLocal<TimeSpan>(() => TimeSpan.Zero);

        /// <summary>
        /// The synchronized current timestamp with correction offset added
        /// </summary>
        public DateTime LocalTimeNow => DateTime.Now + Offset.Value;

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
                Offset.Value = ntp.GetCorrectionOffset();
            }
        }
    }
}
