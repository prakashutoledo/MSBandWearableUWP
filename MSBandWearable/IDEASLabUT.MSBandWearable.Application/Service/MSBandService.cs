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
        private static readonly Lazy<ILogger> LoggerInstance = new Lazy<ILogger>(() => MSBandWearableUtil.LoggerFactory.CreateLogger());

        // Lazy singleton pattern
        public static MSBandService Singleton => Instance.Value;

        private MSBandService()
        {
            // private initialization
        }

        public BandStatus BandStatus { get; set; } = BandStatus.UNKNOWN;

        public IBandClient BandClient { get; private set; }

        public AccelerometerSensor Accelerometer { get; } = new AccelerometerSensor(LoggerInstance.Value);

        public GSRSensor Gsr { get; } = new GSRSensor(LoggerInstance.Value);

        public GyroscopeSensor Gyroscope { get; } = new GyroscopeSensor(LoggerInstance.Value);

        public HeartRateSensor HeartRate { get; } = new HeartRateSensor(LoggerInstance.Value);

        public TemperatureSensor Temperature { get; } = new TemperatureSensor(LoggerInstance.Value);

        public RRIntervalSensor RRInterval { get; } = new RRIntervalSensor(LoggerInstance.Value);

        public string BandName { get; private set; }

        public async Task ConnectBand(string bandName, int selectedIndex)
        {
            BandStatus bandStatus = BandStatus.UNKNOWN;
            try
            {

                var pairedBands = await BandClientManager.Instance.GetBandsAsync().ConfigureAwait(false);
                BandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[selectedIndex]).ConfigureAwait(false);
                bandStatus = BandStatus.Connected;
                BandName = bandName;
            }
            // Catching base exception is a bad practice but it won't allow multiple exception in a single catch
            // Thus, all exceptions will cause band status to be in error status
            catch (Exception)
            {
                bandStatus = BandStatus.Error;
                throw;
            }
            finally
            {
                BandStatus = bandStatus;
            }
        }

        public async Task SubscribeSensors()
        {
            if(BandStatus.Connected != BandStatus)
            {
                return;
            }

            await Accelerometer.Subscribe().ConfigureAwait(false);
            await Gsr.Subscribe().ConfigureAwait(false);
            await Gyroscope.Subscribe().ConfigureAwait(false);
            await HeartRate.Subscribe().ConfigureAwait(false);
            await RRInterval.Subscribe().ConfigureAwait(false);
            await Temperature.Subscribe().ConfigureAwait(false);
            await BandClient.NotificationManager.VibrateAsync(VibrationType.NotificationOneTone).ConfigureAwait(false);
            BandStatus = BandStatus.Subscribed;
        }

        public async Task<IEnumerable<string>> GetPairedBands()
        {
            var devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true)).AsTask().ConfigureAwait(false);
            return devices.Where(device => device.Name.StartsWith("MSFT Band 2")).Select(device => device.Name);
        }
    }
}
