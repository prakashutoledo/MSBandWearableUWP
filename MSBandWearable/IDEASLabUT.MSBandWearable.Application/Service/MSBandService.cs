using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.ViewModel;
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
        private static readonly Lazy<MSBandService> Instance = new Lazy<MSBandService>(() => new MSBandService());

        // Lazy singleton pattern
        public static MSBandService Singleton => Instance.Value;

        private MSBandService()
        {
            // private initialization
        }

        public BandStatus BandStatus { get; set; } = BandStatus.NO_PAIR_BAND_FOUND;
        public IBandClient BandClient { get; private set; }
        public AccelerometerSensor Accelerometer { get; } = new AccelerometerSensor();
        public GSRSensor Gsr { get; } = new GSRSensor();
        public GyroscopeSensor Gyroscope { get; } = new GyroscopeSensor();
        public HeartRateSensor HeartRate { get; } = new HeartRateSensor();
        public TemperatureSensor Temperature { get; } = new TemperatureSensor();

        public async Task ConnectBand()
        {
            try
            {
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync().ConfigureAwait(false);
                BandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]).ConfigureAwait(false);
                BandStatus = BandStatus.SYNCED_SUSCRIBING;
            }
            catch (BandAccessDeniedException)
            {
                BandStatus = BandStatus.NO_SYNC_PERMISSION;
                throw;
            }
            catch (BandIOException)
            {
                BandStatus = BandStatus.BAND_IO_EXCEPTION;
                throw;
            }
            catch (Exception)
            {
                BandStatus = BandStatus.SYNC_ERROR;
                throw;
            }
        }

        public async Task SubscribeSensors()
        {
            await Accelerometer.Subscribe();
            await Gsr.Subscribe();
            await Gyroscope.Subscribe();
            await HeartRate.Subscribe();
            await Temperature.Subscribe();
        }

        public async Task<IOrderedEnumerable<string>> GetPairedBands()
        {
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true)).AsTask().ConfigureAwait(false);
            return devices.Where(device => device.Name.StartsWith("MSFT Band 2")).Select(device => device.Name).OrderBy(device => device);
        }
    }
}
