using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.ViewModel;
using Microsoft.Band;
using Microsoft.Band.Personalization;
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

        public BandStatus BandStatus { get; set; } = BandStatus.NO_PAIR_BAND_FOUND;
        public IBandClient BandClient { get; private set; }
        public AccelerometerSensor Accelerometer { get; } = new AccelerometerSensor();
        public GSRSensor Gsr { get; } = new GSRSensor();
        public GyroscopeSensor Gyroscope { get; } = new GyroscopeSensor();
        public HeartRateSensor HeartRate { get; } = new HeartRateSensor();
        public TemperatureSensor Temperature { get; } = new TemperatureSensor();
        public RRIntervalSensor RRInterval { get; } = new RRIntervalSensor();
        public BandImage BandBackgroundImage { get; private set; }
        public string BandName { get; private set; }

        public async Task ConnectBand(string bandName, int selectedIndex)
        {
            try
            {

                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync().ConfigureAwait(false);
                BandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[selectedIndex]).ConfigureAwait(false);
                BandStatus = BandStatus.SYNCED_SUSCRIBING;
                BandName = bandName;
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
            await Accelerometer.Subscribe().ConfigureAwait(false);
            await Gsr.Subscribe().ConfigureAwait(false);
            await Gyroscope.Subscribe().ConfigureAwait(false);
            await HeartRate.Subscribe().ConfigureAwait(false);
            await RRInterval.Subscribe().ConfigureAwait(false);
            await Temperature.Subscribe().ConfigureAwait(false);
            BandBackgroundImage = await BandClient.PersonalizationManager.GetMeTileImageAsync().ConfigureAwait(false);
            await BandClient.NotificationManager.VibrateAsync(Microsoft.Band.Notifications.VibrationType.NotificationOneTone).ConfigureAwait(false);
            BandStatus = BandStatus.SYNCED;
        }

        public async Task<IEnumerable<string>> GetPairedBands()
        {
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true)).AsTask().ConfigureAwait(false);
            return devices.Where(device => device.Name.StartsWith("MSFT Band 2")).Select(device => device.Name);
        }
    }
}
