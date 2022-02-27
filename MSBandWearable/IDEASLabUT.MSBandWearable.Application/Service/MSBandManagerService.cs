using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Util;
using IDEASLabUT.MSBandWearable.Application.ViewModel;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class MSBandManagerService
    {
        private static readonly Lazy<MSBandManagerService> Instance = new Lazy<MSBandManagerService>(() => new MSBandManagerService(LoggerInstance.Value, MSBandService.Singleton, SubjectViewService.Singleton, NtpSyncService.Singleton));
        private static readonly Lazy<ILogger> LoggerInstance = new Lazy<ILogger>(() => MSBandWearableUtil.LoggerFactory.CreateLogger());
        // Lazy singleton pattern
        public static MSBandManagerService Singleton => Instance.Value;

        private readonly MSBandService msBandService;

        public BandStatus BandStatus { get; private set; } = BandStatus.UNKNOWN;
        public string BandName { get; private set; }
        public AccelerometerSensor Accelerometer { get; private set; }
        public GSRSensor Gsr { get; private set; }
        public GyroscopeSensor Gyroscope { get; private set; }
        public HeartRateSensor HeartRate { get; private set; }
        public TemperatureSensor Temperature { get; private set; }
        public RRIntervalSensor RRInterval { get; private set; }

        private MSBandManagerService(ILogger logger, MSBandService msBandService, SubjectViewService subjectViewService, NtpSyncService ntpSyncService)
        {
            // private initialization
            this.msBandService = msBandService;

            Accelerometer = new AccelerometerSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Gsr = new GSRSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Gyroscope  = new GyroscopeSensor(logger, msBandService, subjectViewService, ntpSyncService);
            HeartRate  = new HeartRateSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Temperature  = new TemperatureSensor(logger, msBandService, subjectViewService, ntpSyncService);
            RRInterval = new RRIntervalSensor(logger, msBandService, subjectViewService, ntpSyncService);
        }

        public async Task ConnectBand(int selectedIndex, string bandName)
        {
            var bandStatus = BandStatus.UNKNOWN;
            try
            {
                await msBandService.ConnectBand(selectedIndex);
                BandName = bandName;
                bandStatus = BandStatus.Connected;
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
            await Accelerometer.Subscribe().ConfigureAwait(false);
            await Gsr.Subscribe().ConfigureAwait(false);
            await Gyroscope.Subscribe().ConfigureAwait(false);
            await HeartRate.Subscribe().ConfigureAwait(false);
            await RRInterval.Subscribe().ConfigureAwait(false);
            await Temperature.Subscribe().ConfigureAwait(false);
            BandStatus = BandStatus.Subscribed;
        }

        public async Task<IEnumerable<string>> GetPairedBands()
        {
            var devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true)).AsTask().ConfigureAwait(false);
            return devices.Where(device => device.Name.StartsWith("MSFT Band 2")).Select(device => device.Name);
        }
    }
}
