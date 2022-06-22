using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// An interface for providing timestamp synchronization service using ntp client
    /// </summary>
    public interface INtpSyncService
    {
        /// <summary>
        /// A correction offset to current date time once synced to ntp pool
        /// </summary>
        TimeSpan CorrectionOffset { get; set; }

        /// <summary>
        /// The synchronized current timestamp with correction offset added
        /// </summary>
        DateTime LocalTimeNow { get; }

        bool Synced { get; }

        /// <summary>
        /// Sunchronized the datetime for this application to given ntp pool by finding the datetime correction offset
        /// </summary>
        /// <param name="poolAddress">A ntp pool to get the correction offset</param>
        /// <returns>A task that can be awaited</returns>
        Task SyncTimestamp(string poolAddress);
    }
}
