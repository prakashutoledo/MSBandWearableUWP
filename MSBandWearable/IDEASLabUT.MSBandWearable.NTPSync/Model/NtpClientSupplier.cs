/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using GuerrillaNtp;

using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// A <see cref="NtpClient"/ supplier>
    /// </summary>
    internal class NtpClientSupplier : INtpClientSupplier
    {
        /// <summary>
        /// Supply the ntp client for given ip address. This doesn't handle any exception if ntp client is failed to create.
        /// This doesn't dispose any managed or unmanaged resource held by ntp client. Caller is responsible for to dispose the
        /// resource
        /// </summary>
        /// <param name="ntpPool">An ntp pool to resolve ip address</param>
        /// <returns>A newly created ntp client</returns>
        /// <exception cref="ArgumentNullException">if given ip address is null</exception>
        public async Task<NtpClient> Supply(string ntpPool)
        {
            if (ntpPool == null)
            {
                throw new ArgumentNullException(nameof(ntpPool));
            }
            // Only used the first address from the given pool
            var ipAddresses = await Dns.GetHostAddressesAsync(ntpPool).ConfigureAwait(false);

            if (ipAddresses == null || !ipAddresses.Any())
            {
                return null;
            }
            return new NtpClient(ipAddresses.First());
        }
    }
}
