using IDEASLabUT.MSBandWearable.Core.Service;

using Microsoft.Band;

using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    /// <summary>
    /// A MS Band 2 client service to connect to band and subscribe available supported sensors using <see cref="IBandClientManager"/>
    /// </summary>
    public class MSBandClientService : IBandClientService
    {
        private static readonly Lazy<MSBandClientService> Instance = new Lazy<MSBandClientService>(() => new MSBandClientService(BandClientManager.Instance));

        // Lazy singleton pattern
        public static MSBandClientService Singleton => Instance.Value;

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
            BandClient = await bandClientManager.ConnectAsync(pairedBands[selectedIndex]);
        }
    }
}
