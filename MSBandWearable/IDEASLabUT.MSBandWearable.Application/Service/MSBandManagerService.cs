using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Util;
using IDEASLabUT.MSBandWearable.ViewModel;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

using static IDEASLabUT.MSBandWearable.Model.BandStatus;
using static IDEASLabUT.MSBandWearable.Util.TaskUtil;
using static Microsoft.Band.Notifications.VibrationType;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A service class for managing supported sensors, see the status of conneted MS Band 2 client using  <see cref="MSBandClientService"/>
    /// </summary>
    public class MSBandManagerService : IBandManagerService
    {
        private static readonly Lazy<MSBandManagerService> Instance = new Lazy<MSBandManagerService>(() => new MSBandManagerService(MSBandWearableApplicationUtil.Logger, MSBandClientService.Singleton, SubjectViewService.Singleton, NtpSyncService.Singleton));
        private const string MSBandNamePrefix = "MSFT Band 2";

        // Lazy singleton pattern
        public static MSBandManagerService Singleton => Instance.Value;

        private readonly IBandClientService msBandService;

        /// <summary>
        /// Initializes a new instance of <see cref="MSBandManagerService"/>
        /// </summary>
        /// <param name="logger">A logger to set</param>
        /// <param name="msBandService">A MS band service to set</param>
        /// <param name="subjectViewService">A subject view service to set</param>
        /// <param name="ntpSyncService">A ntp sync service to set</param>
        /// <exception cref="ArgumentNullException">If msBandService is null</exception>
        private MSBandManagerService(ILogger logger, IBandClientService msBandService, ISubjectViewService subjectViewService, INtpSyncService ntpSyncService)
        {
            // private initialization
            this.msBandService = msBandService ?? throw new ArgumentNullException(nameof(msBandService));
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
        public BandStatus BandStatus { get; set; }

        /// <summary>
        /// The unique name of the connected MS Band 2
        /// </summary>
        public string BandName { get; set; }

        /// <summary>
        /// An accelerometer sensor of the connected MS Band 2
        /// </summary>
        public AccelerometerSensor Accelerometer { get; set; }

        /// <summary>
        /// A gsr sensor of the connected MS Band 2
        /// </summary>
        public GSRSensor Gsr { get; set; }

        /// <summary>
        /// A gyroscope sensor of the connected MS Band 2
        /// </summary>
        public GyroscopeSensor Gyroscope { get; set; }

        /// <summary>
        /// A heart rate sensor of the connected MS Band 2
        /// </summary>
        public HeartRateSensor HeartRate { get; set; }

        /// <summary>
        /// A temperature sensor of the connected MS Band 2
        /// </summary>
        public TemperatureSensor Temperature { get; set; }

        /// <summary>
        /// A rr interval sensor of the connected MS Band 2
        /// </summary>
        public RRIntervalSensor RRInterval { get; set; }

        /// Connects the given selected index from the available paired MS bands with given name
        /// </summary>
        /// <param name="selectedIndex">A selected index of a paired bands</param>
        /// <param name="bandName">A name of the band to connect</param>
        /// <returns>A task that can be awaited</returns>
        public async Task ConnectBand(int selectedIndex, string bandName)
        {
            var bandStatus = UNKNOWN;
            BandName = bandName ?? throw new ArgumentNullException(nameof(bandName));
            try
            {
                await msBandService.ConnectBand(selectedIndex);
                bandStatus = Connected;
            }
            catch (Exception)
            {
                bandStatus = Error;
                throw;
            }
            finally
            {
                BandStatus = bandStatus;
            }
        }

        /// <summary>
        /// Subscribe all available sensors of connected MS Band 2 client
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task SubscribeSensors()
        {
            await Task.WhenAll(Accelerometer.Subscribe(), Gsr.Subscribe(), Gyroscope.Subscribe(), HeartRate.Subscribe(), RRInterval.Subscribe(), Temperature.Subscribe())
                    .ContinueWithStatus()
                    .ContinueWithStatusSupplier(subscribeTask => StatusTask(subscribeTask))
                    .ContinueWithAction(bandStatus => BandStatus = bandStatus);
        }

        private Task<BandStatus> StatusTask(Task<bool> subscriptionTasks)
        {
            if (subscriptionTasks.IsCompletedWithSuccess() && subscriptionTasks.Result)
            {
                var notificationManager = msBandService.BandClient.NotificationManager;
                return notificationManager.VibrateAsync(NotificationTwoTone).ContinueWith(task => Task.FromResult(Subscribed)).Unwrap();
            }
            return Task.FromResult(Error);
        }

        /// <summary>
        /// Find all the MS Band 2 clients which are paired using bluetooth
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task<IEnumerable<string>> GetPairedBands()
        {
            var devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
            return devices.Where(device => device.Name.StartsWith(MSBandNamePrefix)).Select(device => device.Name);
        }
    }
}
