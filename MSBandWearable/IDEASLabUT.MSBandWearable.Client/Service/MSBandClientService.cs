using Microsoft.Band;

using System;
using System.Threading.Tasks;
using System.Linq;
using Windows.Devices.Enumeration;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A MS Band 2 client service to connect to band and subscribe available supported sensors using <see cref="IBandClientManager"/>
    /// </summary>
    public sealed class MSBandClientService : IBandClientService
    {
        private static readonly Lazy<MSBandClientService> Instance = new Lazy<MSBandClientService>(() => new MSBandClientService(BandClientManager.Instance));

        // Lazy singleton pattern
        internal static MSBandClientService Singleton => Instance.Value;

        private readonly IBandClientManager bandClientManager;

        /// <summary>
        /// Initializes a new instance of <see cref="MSBandClientService"/>
        /// </summary>
        /// <param name="bandClientManager">A band client manager to set</param>
        /// <exception cref="ArgumentNullException">If bandClientManager is null</exception>
        private MSBandClientService(IBandClientManager bandClientManager)
        {
            // private initialization
            this.bandClientManager = bandClientManager ?? throw new ArgumentNullException(nameof(bandClientManager));
        }

        /// <summary>
        /// A connected MS Band 2 client
        /// </summary>
        public IBandClient BandClient { get; set; }

        /// <summary>
        /// Connects the given selected index from the available paired MS bands
        /// </summary>
        /// <param name="selectedIndex">A selected index of a paired bands</param>
        /// <returns>A task that can be awaited</returns>
        public async Task ConnectBand(int selectedIndex)
        {
            var pairedBands = await bandClientManager.GetBandsAsync();
            
            if (pairedBands == null)
            {
                throw new NullReferenceException(nameof(pairedBands));
            }

            if (selectedIndex < 0 || selectedIndex >= pairedBands.Length)
            {
                throw new ArgumentOutOfRangeException($"{nameof(selectedIndex)} should be between 0 and {pairedBands.Length} exclusive");
            }
            var firstBand = pairedBands[selectedIndex];
            var deviceInfo = firstBand.GetType().GetProperty("Peer").GetValue(firstBand) as DeviceInformation;
            BandClient = await bandClientManager.ConnectAsync(pairedBands[selectedIndex]);
        }
    }
}
