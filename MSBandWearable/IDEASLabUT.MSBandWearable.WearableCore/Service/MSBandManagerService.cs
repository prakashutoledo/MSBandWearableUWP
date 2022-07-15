/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Sensor;
using IDEASLabUT.MSBandWearable.Util;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

using static IDEASLabUT.MSBandWearable.Extension.TaskExtension;
using static IDEASLabUT.MSBandWearable.Model.BandStatus;
using static Microsoft.Band.Notifications.VibrationType;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A service class for managing supported sensors, see the status of conneted MS Band 2 client using  <see cref="MSBandClientService"/>
    /// </summary>
    public class MSBandManagerService : IBandManagerService
    {
        private static readonly Lazy<MSBandManagerService> Instance = new Lazy<MSBandManagerService>(() => new MSBandManagerService(SerilogLoggerUtil.Logger, MSBandClientService.Singleton, SubjectViewService.Singleton, NtpSyncService.Singleton));
        private const string MSBandNamePrefix = "MSFT Band 2";

        // Lazy singleton pattern
        internal static MSBandManagerService Singleton => Instance.Value;

        private readonly IBandClientService msBandService;

        /// <summary>
        /// Initializes a new instance of <see cref="MSBandManagerService"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp sync service to set</param>
        private MSBandManagerService(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService)
        {
            // private initialization
            this.msBandService = msBandService;
            BandStatus = UNKNOWN;
            Accelerometer = new AccelerometerSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Gsr = new GSRSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Gyroscope = new GyroscopeSensor(logger, msBandService, subjectViewService, ntpSyncService);
            HeartRate = new HeartRateSensor(logger, msBandService, subjectViewService, ntpSyncService);
            Temperature = new TemperatureSensor(logger, msBandService, subjectViewService, ntpSyncService);
            RRInterval = new RRIntervalSensor(logger, msBandService, subjectViewService, ntpSyncService);
        }

        /// <summary>
        /// The current status of the connected MS Band 2
        /// </summary>
        public BandStatus BandStatus { get; private set; }

        /// <summary>
        /// The unique name of the connected MS Band 2
        /// </summary>
        public string BandName { get; private set; }

        /// <summary>
        /// An accelerometer sensor of the connected MS Band 2
        /// </summary>
        public AccelerometerSensor Accelerometer { get; }

        /// <summary>
        /// A gsr sensor of the connected MS Band 2
        /// </summary>
        public GSRSensor Gsr { get; }

        /// <summary>
        /// A gyroscope sensor of the connected MS Band 2
        /// </summary>
        public GyroscopeSensor Gyroscope { get; }

        /// <summary>
        /// A heart rate sensor of the connected MS Band 2
        /// </summary>
        public HeartRateSensor HeartRate { get; }

        /// <summary>
        /// A temperature sensor of the connected MS Band 2
        /// </summary>
        public TemperatureSensor Temperature { get; }

        /// <summary>
        /// A rr interval sensor of the connected MS Band 2
        /// </summary>
        public RRIntervalSensor RRInterval { get; }

        /// Connects the given selected index from the available paired MS bands with given name
        /// </summary>
        /// <param name="selectedIndex">A selected index of a paired bands</param>
        /// <param name="bandName">A name of the band to connect</param>
        /// <returns>A task that can be awaited</returns>
        public async Task ConnectBand(string bandName)
        {
            await msBandService.ConnectBand(bandName)
                    .ContinueWithSupplier(previousTask => ToBandStatusTask(previousTask))
                    .ContinueWithAction(bandStatus => BandStatus = bandStatus)
                    .ContinueWithAction(() => BandName = BandStatus == Connected ? bandName : null);
        }

        /// <summary>
        /// Subscribe all available sensors of connected MS Band 2 client
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public Task SubscribeSensors()
        {
            var subscriptionTasks = Task.WhenAll(
                Accelerometer.Subscribe(),
                Gsr.Subscribe(),
                Gyroscope.Subscribe(),
                HeartRate.Subscribe(),
                RRInterval.Subscribe(),
                Temperature.Subscribe()
            );

            return subscriptionTasks.ContinueWithStatus()
                 .ContinueWithSupplier(subscribeTask => VibrateAndSubscribe(subscribeTask))
                 .ContinueWithAction(bandStatus => BandStatus = bandStatus);
        }


        /// <summary>
        /// Unsubscribe all available sensors of connected MS Band 2 client
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public Task UnsubscribeSensors()
        {
            var unsubscribeTasks = Task.WhenAll(
                Accelerometer.Subscribe(),
                Gsr.Subscribe(),
                Gyroscope.Subscribe(),
                HeartRate.Subscribe(),
                RRInterval.Subscribe(),
                Temperature.Subscribe()
            );

            return unsubscribeTasks
                .ContinueWithStatus()
                .ContinueWithAction(() => BandStatus = UnSubscribed);
        }


        /// <summary>
        /// Find all the MS Band 2 clients which are paired using bluetooth
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task<IEnumerable<string>> GetPairedBands()
        {
            var devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
            return devices.Select(device => device.Name).Where(deviceName => deviceName.StartsWith(MSBandNamePrefix));
        }

        private Task<BandStatus> ToBandStatusTask(Task task)
        {
            return Task.FromResult(task.IsCompletedWithSuccess() ? Connected : Error);
        }

        private Task<BandStatus> VibrateAndSubscribe(Task<bool> subscriptionTask)
        {
            if (subscriptionTask.IsCompletedWithSuccess() && subscriptionTask.Result)
            {
                var notificationManager = msBandService.BandClient.NotificationManager;
                return notificationManager.VibrateAsync(NotificationTwoTone).ContinueWithSupplier(task => Task.FromResult(Subscribed));
            }
            return Task.FromResult(Error);
        }
    }
}
