using Microsoft.Band;
using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class MSBandClientService : IBandClientService
    {
        private static readonly Lazy<MSBandClientService> Instance = new Lazy<MSBandClientService>(() => new MSBandClientService(BandClientManager.Instance));

        // Lazy singleton pattern
        public static MSBandClientService Singleton => Instance.Value;

        private readonly IBandClientManager bandClientManager;

        private MSBandClientService(IBandClientManager bandClientManager)
        {
            // private initialization
            this.bandClientManager = bandClientManager;
        }

        public IBandClient BandClient { get; set; }

        public async Task ConnectBand(int selectedIndex)
        {
            var pairedBands = await bandClientManager.GetBandsAsync();
            BandClient = await bandClientManager.ConnectAsync(pairedBands[selectedIndex]);
        }
    }
}
