using IDEASLabUT.MSBandWearable.Core.Service;
using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Application.Util;
using IDEASLabUT.MSBandWearable.Core.ViewModel;

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
    public class MSBandManagerService : IBandManagerService
    {
        private static readonly Lazy<MSBandManagerService> Instance = new Lazy<MSBandManagerService>(() => new MSBandManagerService(MSBandWearableUtil.Logger, MSBandClientService.Singleton, SubjectViewService.Singleton, NtpSyncService.Singleton));
        private static readonly string MSBandNamePrefix = "MSFT Band 2";

        // Lazy singleton pattern
        public static MSBandManagerService Singleton => Instance.Value;

        private readonly IBandClientService msBandService;

        public BandStatus BandStatus { get; set; }
        public string BandName { get; set; }
        public AccelerometerSensor Accelerometer { get; set; }
        public GSRSensor Gsr { get; set; }
        public GyroscopeSensor Gyroscope { get; set; }
        public HeartRateSensor HeartRate { get; set; }
        public TemperatureSensor Temperature { get; set; }
        public RRIntervalSensor RRInterval { get; set; }

        private MSBandManagerService(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService)
        {
            // private initialization
            this.msBandService = msBandService ?? throw new ArgumentNullException(nameof(msBandService));
            BandStatus = BandStatus.UNKNOWN;
            Accelerometer = new AccelerometerSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Gsr = new GSRSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Gyroscope = new GyroscopeSensor(logger, msBandService, subjectViewService, ntpSyncService);
            HeartRate = new HeartRateSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Temperature = new TemperatureSensor(logger, msBandService, subjectViewService, ntpSyncService);
            RRInterval = new RRIntervalSensor(logger, msBandService, subjectViewService, ntpSyncService);
        }

        public async Task ConnectBand(int selectedIndex, string bandName)
        {
            var bandStatus = BandStatus.UNKNOWN;
            try
            {
                await msBandService.ConnectBand(selectedIndex);
                BandName = bandName ?? throw new ArgumentNullException(nameof(bandName));
                bandStatus = BandStatus.Connected;
            }
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
            await Accelerometer.Subscribe();
            await Gsr.Subscribe();
            await Gyroscope.Subscribe();
            await HeartRate.Subscribe();
            await RRInterval.Subscribe();
            await Temperature.Subscribe();
            await msBandService.BandClient.NotificationManager.VibrateAsync(VibrationType.NotificationTwoTone);
            BandStatus = BandStatus.Subscribed;
        }

        public async Task<IEnumerable<string>> GetPairedBands()
        {
            var devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
            return devices.Where(device => device.Name.StartsWith(MSBandNamePrefix)).Select(device => device.Name);
        }
    }
}
