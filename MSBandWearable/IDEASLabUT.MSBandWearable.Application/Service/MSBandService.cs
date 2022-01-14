using IDEASLabUT.MSBandWearable.Application.Domain;
using Microsoft.Band;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class MSBandService
    {
        static MSBandService()
        {
            if (Singleton == null)
            {
                Singleton = new MSBandService();
            }
        }

        public static MSBandService Singleton { get; private set; } = null;

        private MSBandService()
        {
            // private constructor
        }

        public BandStatus BandStatus { get; set; } = BandStatus.NO_PAIR_BAND_FOUND;
        public IBandClient BandClient { get; private set; }

        public async Task ConnectBand()
        {
            try
            {
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync().ConfigureAwait(false);
                BandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]).ConfigureAwait(false);
                BandStatus = BandStatus.SYNCED_SUSCRIBING;
            }
            catch(BandAccessDeniedException)
            {
                BandStatus = BandStatus.NO_SYNC_PERMISSION;
                throw;
            }
            catch(BandIOException)
            {
                BandStatus = BandStatus.BAND_IO_EXCEPTION;
                throw;
            }
            catch(Exception)
            {
                BandStatus = BandStatus.SYNC_ERROR;
                throw;
            }
        }

        public async Task<IOrderedEnumerable<string>> GetPairedBands()
        {
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true)).AsTask().ConfigureAwait(false);
            return devices.Where(device => device.Name.StartsWith("MSFT Band 2")).Select(device => device.Name).OrderBy(device => device);
        }
    }
}
