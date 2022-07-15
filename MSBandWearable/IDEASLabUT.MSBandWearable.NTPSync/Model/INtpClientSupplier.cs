/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using GuerrillaNtp;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// A supplier for <see cref="NtpClient"/>
    /// </summary>
    public interface INtpClientSupplier
    {
        /// <summary>
        /// Supplies the NtpClient
        /// </summary>
        /// <param name="ntpPool">A ntp pool for the client to supply</param>
        /// <returns></returns>
        Task<NtpClient> Supply(string ntpPool);
    }
}
