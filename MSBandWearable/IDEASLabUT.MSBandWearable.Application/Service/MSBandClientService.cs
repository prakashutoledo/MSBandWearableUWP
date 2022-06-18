using Microsoft.Band;

using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A MS Band 2 client service to connect to band and subscribe available supported sensors using <see cref="IBandClientManager"/>
    /// </summary>
    public class MSBandClientService : IBandClientService
    {
        private static readonly Lazy<MSBandClientService> Instance = new Lazy<MSBandClientService>(() => new MSBandClientService(BandClientManager.Instance));
        private const string BluetoothDeviceInfoPeer = "Peer";

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
        public async Task ConnectBand(string bandName)
        {
            var pairedBands = await bandClientManager.GetBandsAsync();
            if (pairedBands == null)
            {
                return;
            }

            // This is a hack with the help of reflection to get device information from bandInfo
            // Checks if the band bluetooth id ends with band name last split substring
            bool PairedBandConnectionPredicate(IBandInfo bandInfo)
            {
                var deviceInfo = bandInfo.GetType().GetProperty(BluetoothDeviceInfoPeer).GetValue(bandInfo) as DeviceInformation;
                return deviceInfo.Id.Contains(bandName.Split(' ').Last());
            }

            var toConnect = pairedBands.FirstOrDefault(PairedBandConnectionPredicate);
            BandClient = await bandClientManager.ConnectAsync(toConnect);
        }
    }
}
