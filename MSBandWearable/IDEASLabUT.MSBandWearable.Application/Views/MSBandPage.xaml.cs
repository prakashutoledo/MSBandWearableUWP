﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Model.Notification;
using IDEASLabUT.MSBandWearable.Service;
using IDEASLabUT.MSBandWearable.ViewModel;

using LiveCharts;
using LiveCharts.Configurations;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static IDEASLabUT.MSBandWearable.Model.Notification.PayloadType;
using static IDEASLabUT.MSBandWearable.MSBandWearableApplicationGlobals;
using static IDEASLabUT.MSBandWearable.Util.CoreDispatcherUtil;
using static Microsoft.Band.Sensors.HeartRateQuality;
using static Windows.UI.Colors;

namespace IDEASLabUT.MSBandWearable.Views
{
    /// <summary>
    /// A page for showing Microsoft Band 2 sensors data including continuous time series graphs.
    /// This page also shows connected Empatica E4 serial number, current view in IOS wearable 
    /// application for E4, and current subject id of the subject running IOS wearable application
    /// </summary>
    public sealed partial class MSBandPage
    {
        private static readonly SolidColorBrush ColorBrush = new SolidColorBrush();
        private ServiceFactory ServiceFactory { get; } = ServiceFactory.Singleton;
        private ViewModelFactory ViewModelFactory { get; } = ViewModelFactory.Singleton;

        private ObservableCollection<string> AvailableBands { get; } = new ObservableCollection<string>();
        private DispatcherTimer WebSocketTimer { get; } = new DispatcherTimer();

        // These chart values properties should be public for data binding line series chart
        public ChartValues<DateTimeModel> GsrDataPoint { get; } = new ChartValues<DateTimeModel>();
        public ChartValues<DateTimeModel> IbiDataPoint { get; } = new ChartValues<DateTimeModel>();

        private int connectionCount = 0;

        /// <summary>
        /// Initializes a new instance of <see cref="MSBandPage"/>
        /// </summary>
        public MSBandPage()
        {
            InitializeComponent();
            AddLiveCharts();
            AddDispatchTimersTickIntervals();
            AddSensorValueChangedHandlers();
            SetMessagePostProcessor();
            SetColorBrush();
        }

        private void SetColorBrush()
        {
            heartRatePath.Fill = ColorBrush;
        }

        /// <summary>
        /// Add line charts with model mapper for data binding
        /// </summary>
        private void AddLiveCharts()
        {
            var dateTimeModelMapper = Mappers.Xy<DateTimeModel>()
                .X(model => model.DateTime)
                .Y(model => model.Value);

            Charting.For<DateTimeModel>(dateTimeModelMapper);
        }

        /// <summary>
        /// Creates a dispatch timer and its corresponding call back action for GSR and webSocket connection
        /// </summary>
        private void AddDispatchTimersTickIntervals()
        {
            WebSocketTimer.Interval = TimeSpan.FromMinutes(5);
            WebSocketTimer.Tick += WebSocketTimerOnTick;
        }

        /// <summary>
        /// Add sensor model changed callback handlers for MS Band wearable sensors
        /// </summary>
        private void AddSensorValueChangedHandlers()
        {
            var bandManagerService = ServiceFactory.GetBandManagerService;
            bandManagerService.HeartRate.SensorModelChanged = HeartRateSensorValueChanged;
            bandManagerService.RRInterval.SensorModelChanged = IbiSensorValueChanged;
            bandManagerService.Gsr.SensorModelChanged = GsrValueChanged;
            bandManagerService.Temperature.SensorModelChanged = TemperatureValueChanged;
        }

        /// <summary>
        /// Set webSocket message post processors
        /// </summary>
        private void SetMessagePostProcessor()
        {
            ServiceFactory.GetWebSocketService.AddMessagePostProcessor<EmpaticaE4Band>(E4Band, OnEmpaticaE4BandMessageReceived);
        }

        /// <summary>
        /// An on tick callback for webSocket dispatch timer for closing the current webSocket connection and creating new one
        /// </summary>
        /// <param name="sender">The sender of current timer on tick event</param>
        /// <param name="eventArgs">An event arguments</param>
        private async void WebSocketTimerOnTick(object sender, object eventArgs)
        {
            var websocketService = ServiceFactory.GetWebSocketService;
            websocketService.Close();
            await websocketService.Connect(ServiceFactory.GetPropertiesService.GetProperty(WebSocketConnectionUriJsonKey), WebSocketContinueTask);
        }

        /// <summary>
        /// An async callback for MSBand RR interval sensor value change to run in ui dispatch thread
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        private async Task IbiSensorValueChanged()
        {
            var ibi = ServiceFactory.GetBandManagerService.RRInterval.Model.Ibi;
            var ibiModel = ViewModelFactory.GetRRIntervalModel;
            await RunLaterInUIThread(() =>
            {
                ibiModel.Ibi = ibi;
                IbiDataPoint.Add(new DateTimeModel
                {
                    DateTime = DateTime.Now.Ticks,
                    Value = ibi
                });

                if (IbiDataPoint.Count > 20)
                {
                    IbiDataPoint.RemoveAt(0);
                }
            });
        }

        /// <summary>
        /// An async callback for MSBand heart rate sensor value change event to run in a ui dispatch thread
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        private async Task HeartRateSensorValueChanged()
        {
            var heartRateEvent = ServiceFactory.GetBandManagerService.HeartRate.Model;
            var heartRateStatus = heartRateEvent.HeartRateStatus;
            var bpm = heartRateEvent.Bpm;

            var heartRateModel = ViewModelFactory.GetHeartRateModel;

            await RunLaterInUIThread(() =>
            {
                ColorBrush.Color = White;
                heartRateModel.HeartRateStatus = heartRateStatus == Locked;
                heartRateModel.Bpm = bpm;
                if (bpm > heartRateModel.MaxBpm)
                {
                    heartRateModel.MaxBpm = bpm;
                }

                if (bpm < heartRateModel.MinBpm)
                {
                    heartRateModel.MinBpm = bpm;
                }
            });
        }

        private async Task GsrValueChanged()
        {
            var gsrModel = ViewModelFactory.GetGSRModel;
            var gsr = ServiceFactory.GetBandManagerService.Gsr.Model.Gsr;
            await RunLaterInUIThread(() =>
            {
                gsrModel.Gsr = gsr;
                GsrDataPoint.Add(new DateTimeModel
                {
                    DateTime = DateTime.Now.Ticks,
                    Value = gsr
                });

                if (GsrDataPoint.Count > 20)
                {
                    GsrDataPoint.RemoveAt(0);
                }
            });
        }

        private async Task TemperatureValueChanged()
        {
            var temperatureModel = ViewModelFactory.GetTemperatureModel;
            var temperature = ServiceFactory.GetBandManagerService.Temperature.Model.Temperature;

            await RunLaterInUIThread(() => temperatureModel.Temperature = temperature);
        }

        /// <summary>
        /// A callback when current page is loaded to user
        /// </summary>
        /// <param name="sender">The sender of the current page loaded event</param>
        /// <param name="routedEventArgs">A routed event arguments</param>
        private async void PageLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await SearchBands();
        }

        /// <summary>
        /// An action callback when search band button is clicked
        /// </summary>
        /// <param name="sender">The sender of the search band button clicked event</param>
        /// <param name="routedEventArgs">A routed event arguments</param>
        private async void SearchBandButtonAction(object sender, RoutedEventArgs routedEventArgs)
        {
            // Load available paired bands
            await LoadBands();
        }

        /// <summary>
        /// A callback for Empatica E4 Band webSocket message received
        /// </summary>
        /// <param name="empaticaE4Band">A webSocket message containing Empatica E4 Band details</param>
        /// <returns>A task that can be awaited</returns>
        private async Task OnEmpaticaE4BandMessageReceived(EmpaticaE4Band empaticaE4Band)
        {
            if (empaticaE4Band == null)
            {
                return;
            }
            var subjectViewService = ServiceFactory.GetSubjectViewService;
            subjectViewService.CurrentView = empaticaE4Band.FromView;
            subjectViewService.SubjectId = empaticaE4Band.SubjectId;

            var subjectViewModel = ViewModelFactory.GetSubjectViewModel;
            await RunLaterInUIThread(() =>
            {
                subjectViewModel.CurrentView = empaticaE4Band.FromView;
                subjectViewModel.SubjectId = empaticaE4Band.SubjectId;

                if (empaticaE4Band.Device.Connected)
                {
                    subjectViewModel.E4SerialNumber = empaticaE4Band.Device.SerialNumber;
                }
            });
        }

        /// <summary>
        /// Loads the currently paired MS Band wearables to this page
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        private async Task LoadBands()
        {
            commandBar.Visibility = Visibility.Collapsed;
            await HideAllGridsWithMessage("Loading Paired Bands", true);
            await Task.Delay(200);
            var availableBandNames = await ServiceFactory.GetBandManagerService.GetPairedBands();
            if (!availableBandNames.Any())
            {
                var messageDialog = new MessageDialog("No Paired Bands Available!");
                messageDialog.Commands.Add(new UICommand("Pair Your Band Now! Make Sure Your Band Is Paired In Bluetooth Mode", new UICommandInvokedHandler(CommandInvokedHandler), 1));
                messageDialog.Commands.Add(new UICommand("Not Now", new UICommandInvokedHandler(CommandInvokedHandler), -1));
                _ = await messageDialog.ShowAsync();
                await SearchBands();
            }
            else
            {
                await RunLaterInUIThread(() =>
                {
                    AvailableBands.Clear();
                    foreach (var device in availableBandNames)
                    {
                        AvailableBands.Add(device);
                    }
                    availableBandGrid.Visibility = Visibility.Visible;
                });
            }
        }

        /// <summary>
        /// A callback for command invoked action
        /// </summary>
        /// <param name="command">A ui command that has been inkoked</param>
        private async void CommandInvokedHandler(IUICommand command)
        {
            if (1.Equals(command.Id))
            {
                _ = await Launcher.LaunchUriAsync(new Uri("ms-settings:bluetooth"));
            }
        }

        /// <summary>
        /// Hides all available grids (search band and available band grid) from this page to show progress ring with given message
        /// </summary>
        /// <param name="message">A message to set</param>
        /// <param name="isProgress">A flag which sets the progress ring</param>
        /// <returns></returns>
        private async Task HideAllGridsWithMessage(string message, bool isProgress = true)
        {
            await RunLaterInUIThread(() =>
            {
                searchBandGrid.Visibility = Visibility.Collapsed;
                availableBandGrid.Visibility = Visibility.Collapsed;
                syncProgressRing.IsActive = isProgress;
                syncTextBlock.Text = message;
                syncGrid.Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// A callback when current page is navigated from other page
        /// </summary>
        /// <param name="navigationEventArgs">A navigation event arguments</param>
        protected override async void OnNavigatedTo(NavigationEventArgs navigationEventArgs)
        {
            base.OnNavigatedTo(navigationEventArgs);
            await Task.CompletedTask;
        }

        /// <summary>
        /// An action callback when start or stop session button is clicked
        /// </summary>
        /// <param name="sender">The sender of current button clicked event</param>
        /// <param name="routedEventArgs">A routed event arguments</param>
        private async void StartOrStopSessionButttonAction(object sender, RoutedEventArgs routedEventArgs)
        {
            var symbolIcon = new SymbolIcon(Symbol.Pause);
            var label = "Pause Session";
            var sessionInProgress = true;
            var subjectViewService = ServiceFactory.GetSubjectViewService;
            if (subjectViewService.SessionInProgress)
            {
                symbolIcon = new SymbolIcon(Symbol.Play);
                label = "Resume Session";
                sessionInProgress = false;
            }

            subjectViewService.SessionInProgress = sessionInProgress;
            await RunLaterInUIThread(() =>
            {
                startOrStopSessionButtton.Icon = symbolIcon;
                startOrStopSessionButtton.Label = label;
            });
        }

        /// <summary>
        /// Sets the visibility of search band grid to true and hides available band grid
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        private async Task SearchBands()
        {
            await RunLaterInUIThread(() =>
            {
                searchBandGrid.Visibility = Visibility.Collapsed;
                availableBandGrid.Visibility = Visibility.Collapsed;
                searchBandGrid.Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// An action callback when paired MS band is selected from the available bands combo box
        /// </summary>
        /// <param name="sender">The sender of current combo box selected event</param>
        /// <param name="_">A selected changed event arguments</param>
        private async void BandSelectionChanged(object sender, SelectionChangedEventArgs _)
        {
            await ConnectBand((sender as ComboBox).SelectedValue.ToString());
        }

        /// <summary>
        /// Connects the band with given index and name
        /// </summary>
        /// <param name="bandName">A name of selected band to connect</param>
        /// <returns>A task that can be awaited</returns>
        private async Task ConnectBand(string bandName)
        {
            var messageDialog = new MessageDialog(string.Empty);
            syncStackPanel.Visibility = Visibility.Visible;
            commandBar.IsEnabled = false;

            await HideAllGridsWithMessage($"Connecting to band ({bandName})...");

            var bandManagerService = ServiceFactory.GetBandManagerService;
            await bandManagerService.ConnectBand(bandName);
            switch (bandManagerService.BandStatus)
            {
                case BandStatus.Error:
                case BandStatus.UNKNOWN:
                    messageDialog.Content = $"Failed to connect to Microsoft Band ({bandName}).";
                    messageDialog.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(CommandInvokedHandler), -1));
                    break;
            }
            if (!string.IsNullOrEmpty(messageDialog.Content))
            {
                syncStackPanel.Visibility = Visibility.Collapsed;
                _ = await messageDialog.ShowAsync();
                availableBandGrid.Visibility = Visibility.Visible;
            }
            else
            {
                await StartDashboard();
            }
        }

        /// <summary>
        /// Starts the dashboard for currently loaded page in UI.
        /// This will hides all the grids, sets the visibility of command bar to true, subscribe available
        /// MS Band sensors which is selected, sync timestamp to 'ntp.pool.org', connect to webSocket and start
        /// the gsr and webSocket timers
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        private async Task StartDashboard()
        {
            var bandManagerService = ServiceFactory.GetBandManagerService;
            await HideAllGridsWithMessage($"Preparing Dashboard for Microsoft Band ({bandManagerService.BandName})...");
            await bandManagerService.SubscribeSensors();
            await RunLaterInUIThread(() =>
            {
                commandBar.Visibility = Visibility.Visible;
                ViewModelFactory.GetSubjectViewModel.MSBandSerialNumber = bandManagerService.BandName;
                UpdateCommandBar();
            });
            var propertiesService = ServiceFactory.GetPropertiesService;
            await ServiceFactory.GetNtpSyncService.SyncTimestamp(propertiesService.GetProperty(NtpPoolUriJsonKey));
            await ServiceFactory.GetWebSocketService.Connect(propertiesService.GetProperty(WebSocketConnectionUriJsonKey), WebSocketContinueTask);
        }

        private Task WebSocketContinueTask(bool isConnected)
        {
            if (connectionCount++ == 0 && isConnected)
            {
                WebSocketTimer.Start();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the command bar for MS Band page based of band status of selected band
        /// </summary>
        private void UpdateCommandBar()
        {
            switch (ServiceFactory.GetBandManagerService.BandStatus)
            {
                case BandStatus.Connected:
                //return;
                case BandStatus.Subscribed:
                    syncGrid.Visibility = Visibility.Collapsed;
                    startOrStopSessionButtton.IsEnabled = true;
                    commandBar.IsEnabled = true;
                    commandBar.IsOpen = true;
                    break;

                case BandStatus.UnSubscribed:
                    startOrStopSessionButtton.IsEnabled = false;
                    break;

                case BandStatus.Error:
                case BandStatus.UNKNOWN:
                default:
                    commandBar.IsEnabled = false;
                    break;
            }
        }
    }
}
