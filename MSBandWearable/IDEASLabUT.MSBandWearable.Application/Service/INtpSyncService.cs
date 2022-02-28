using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public interface INtpSyncService
    {
        TimeSpan Offset { get; set; }

        /// <summary>
        /// The synchronized current timestamp with correction offset added
        /// </summary>
        DateTime LocalTimeNow { get; }

        /// <summary>
        /// Sunchronized the datetime for this application to given ntp pool by finding the datetime correction offset
        /// </summary>
        /// <param name="poolAddress">A ntp pool to get the correction offset</param>
        void SyncTimestamp(string poolAddress);
    }
}
