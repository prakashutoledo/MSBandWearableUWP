using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Model.Notification;
using IDEASLabUT.MSBandWearable.Service;
using IDEASLabUT.MSBandWearable.ViewModel;

using LiveCharts;
using LiveCharts.Configurations;

using Microsoft.Band;
using Microsoft.Extensions.Configuration;

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

using static IDEASLabUT.MSBandWearable.MSBandWearableApplicationGlobals;
using static IDEASLabUT.MSBandWearable.Util.MSBandWearableCoreUtil;
using static IDEASLabUT.MSBandWearable.Util.MSBandWearableUtil;
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
        private IBandManagerService BandManagerService { get; } = MSBandManagerService.Singleton;
        private ISubjectViewService SubjectAndViewService { get; } = SubjectViewService.Singleton;
        private WebSocketService SocketService { get; } = WebSocketService.Singleton;
        private SubjectViewModel SubjectAndView { get; } = new SubjectViewModel();
        private ObservableCollection<string> AvailableBands { get; } = new ObservableCollection<string>();
        private DispatcherTimer GsrTimer { get; set; }
        private DispatcherTimer WebSocketTimer { get; set; }

        // These chart values properties should be public for data binding line series chart
        public ChartValues<DateTimeModel> GsrDataPoint { get; } = new ChartValues<DateTimeModel>();
        public ChartValues<DateTimeModel> IbiDataPoint { get; } = new ChartValues<DateTimeModel>();

        private double gsrValue;

        /// <summary>
        /// Initializes a new instance of <see cref="MSBandPage"/>
        /// </summary>
        public MSBandPage()
        {
            InitializeComponent();
            AddLiveCharts();
            AddDispatchTimers();
            AddSensorValueChangedHandlers();
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
        private void AddDispatchTimers()
        {
            GsrTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            GsrTimer.Tick += GsrTimerOnTick;

            WebSocketTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(5)
            };
            WebSocketTimer.Tick += WebSocketTimerOnTick;
        }

        /// <summary>
        /// Add sensor model changed callback handlers for MS Band wearable sensors
        /// </summary>
        private void AddSensorValueChangedHandlers()
        {
            BandManagerService.HeartRate.SensorModelChanged = HeartRateSensorValueChanged;
            BandManagerService.Gsr.SensorModelChanged = GsrSensorValueChanged;
            BandManagerService.RRInterval.SensorModelChanged = IbiSensorValueChanged;
        }

        /// <summary>
        /// An on tick callback for webSocket dispatch timer for closing the current webSocket connection and creating new one
        /// </summary>
        /// <param name="sender">The sender of current timer on tick event</param>
        /// <param name="eventArgs">An event arguments</param>
        private async void WebSocketTimerOnTick(object sender, object eventArgs)
        {
            SocketService.Close();
            await SocketService.Connect(ApplicationProperties.GetValue<string>(WebSocketConnectionUriJsonKey), OnEmpaticaE4BandMessageReceived);
        }

        /// <summary>
        /// An on tick callback for gsr dispatch timer for binding current gsr value to gsr data point line series in a ui
        /// dispatch thread
        /// </summary>
        /// <param name="sender">The sender of current timer on tick event</param>
        /// <param name="eventArgs">An event arguments</param>
        private async void GsrTimerOnTick(object sender, object eventArgs)
        {
            await RunLaterInUIThread(() =>
            {
                GsrDataPoint.Add(new DateTimeModel
                {
                    DateTime = DateTime.Now.Ticks,
                    Value = gsrValue
                });

                if (GsrDataPoint.Count > 20)
                {
                    GsrDataPoint.RemoveAt(0);
                }
            });
        }

        /// <summary>
        /// An async callback for MSBand rr interval sensor value change to run in ui dispatch thread
        /// </summary>
        /// <param name="value">A new rrinterval event value</param>
        /// <returns>A task that can be awaited</returns>
        private async Task IbiSensorValueChanged(RRIntervalEvent value)
        {
            await RunLaterInUIThread(() =>
            {
                IbiDataPoint.Add(new DateTimeModel
                {
                    DateTime = DateTime.Now.Ticks,
                    Value = value.Ibi
                });

                if (IbiDataPoint.Count > 20)
                {
                    IbiDataPoint.RemoveAt(0);
                }
            });
        }

        /// <summary>
        /// An async callback for MSBand GSR sensor value change event to run in ui dispatch thread
        /// </summary>
        /// <param name="value">A new GSR event value</param>
        /// <returns>A task that can be awaited</returns>
        private async Task GsrSensorValueChanged(GSREvent value)
        {
            await RunLaterInUIThread(() =>
            {
                gsrValue = value.Gsr;
            });
        }

        /// <summary>
        /// An async callback for MSBand heart rate sensor value change event to run in a ui dispatch thread
        /// </summary>
        /// <param name="value">A new heart rate event value</param>
        /// <returns>A task that can be awaited</returns>
        private async Task HeartRateSensorValueChanged(HeartRateEvent value)
        {
            await RunLaterInUIThread(() =>
            {
                heartRatePath.Fill = new SolidColorBrush(Locked == BandManagerService.HeartRate.HeartRateStatus ? White : Transparent);
            });
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
            SubjectAndViewService.CurrentView = empaticaE4Band.FromView;
            SubjectAndViewService.SubjectId = empaticaE4Band.SubjectId;

            await RunLaterInUIThread(() =>
            {
                SubjectAndView.CurrentView = empaticaE4Band.FromView;
                SubjectAndView.SubjectId = empaticaE4Band.SubjectId;

                if (empaticaE4Band.Device.Connected)
                {
                    SubjectAndView.E4SerialNumber = empaticaE4Band.Device.SerialNumber;
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
            var availableBandNames = await BandManagerService.GetPairedBands();
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
            switch ((int)command.Id)
            {
                case 1:
                    _ = await Launcher.LaunchUriAsync(new Uri("ms-settings:bluetooth"));
                    break;
                default:
                    break;
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
            await Task.CompletedTask;
            base.OnNavigatedTo(navigationEventArgs);
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

            if (SubjectAndViewService.SessionInProgress)
            {
                symbolIcon = new SymbolIcon(Symbol.Play);
                label = "Resume Session";
                sessionInProgress = false;
            }

            SubjectAndViewService.SessionInProgress = sessionInProgress;
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
        /// <param name="changedEventArgs">A selected changed event arguments</param>
        private async void BandSelectionChanged(object sender, SelectionChangedEventArgs changedEventArgs)
        {
            await ConnectBand(availableBandComboBox.SelectedValue.ToString(), availableBandComboBox.SelectedIndex);
        }

        /// <summary>
        /// Connects the band with given index and name
        /// </summary>
        /// <param name="bandName">A name of selected band to connect</param>
        /// <param name="selectedIndex">An index of selected band to connect</param>
        /// <returns>A task that can be awaited</returns>
        private async Task ConnectBand(string bandName, int selectedIndex)
        {
            var messageDialog = new MessageDialog(string.Empty);
            syncStackPanel.Visibility = Visibility.Visible;
            commandBar.IsEnabled = false;

            await HideAllGridsWithMessage($"Connecting to band ({bandName})...");

            try
            {
                await BandManagerService.ConnectBand(selectedIndex, bandName);
            }
            catch (BandAccessDeniedException)
            {
                messageDialog.Content = $"Microsoft Band ({bandName}) doesn't have a permission to synchorize with this device";
                messageDialog.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(CommandInvokedHandler), -1));
            }
            catch (BandIOException)
            {
                messageDialog.Content = $"Failed to connect to Microsoft Band ({bandName}).";
                messageDialog.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(CommandInvokedHandler), -1));
            }
            catch (Exception ex)
            {
                messageDialog.Content = ex.ToString();
            }
            finally
            {
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
            await HideAllGridsWithMessage($"Preparing Dashboard for Microsoft Band ({BandManagerService.BandName})...");
            await BandManagerService.SubscribeSensors();

            await RunLaterInUIThread(() =>
            {
                commandBar.Visibility = Visibility.Visible;
                SubjectAndView.MSBandSerialNumber = BandManagerService.BandName;
                UpdateCommandBar();
            });

            await NtpSyncService.Singleton.SyncTimestamp(ApplicationProperties.GetValue<string>(NtpPoolUriJsonKey));
            await SocketService.Connect(ApplicationProperties.GetValue<string>(WebSocketConnectionUriJsonKey), OnEmpaticaE4BandMessageReceived);

            GsrTimer.Start();
            WebSocketTimer.Start();
        }

        /// <summary>
        /// Updates the command bar for MS Band page based of band status of selected band
        /// </summary>
        private void UpdateCommandBar()
        {
            switch (BandManagerService.BandStatus)
            {
                case BandStatus.Connected:
                    return;
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
