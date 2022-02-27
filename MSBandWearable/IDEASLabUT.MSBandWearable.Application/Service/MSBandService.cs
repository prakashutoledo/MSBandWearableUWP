using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Util;
using IDEASLabUT.MSBandWearable.Application.ViewModel;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class MSBandService
    {
        private static readonly Lazy<MSBandService> Instance = new Lazy<MSBandService>(() => new MSBandService());

        // Lazy singleton pattern
        public static MSBandService Singleton => Instance.Value;

        private MSBandService()
        {
            // private initialization
        }

        public IBandClient BandClient { get; private set; }

        public async Task ConnectBand(int selectedIndex)
        {
            var pairedBands = await BandClientManager.Instance.GetBandsAsync().ConfigureAwait(false);
            BandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[selectedIndex]).ConfigureAwait(false);
        }
    }
}
