using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;
using static Microsoft.Band.Sensors.HeartRateQuality;
using static Windows.UI.Colors;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using IDEASLabUT.MSBandWearable.Application.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using Microsoft.Band;
using Windows.UI.Xaml.Media;
using System.Collections.Generic;
using System.Diagnostics;
using LiveCharts;
using LiveCharts.Configurations;
using IDEASLabUT.MSBandWearable.Application.Model.Notification;
using Newtonsoft.Json;

namespace IDEASLabUT.MSBandWearable.Application.Views
{
    public class MeasureModel
    {
        public long DateTime { get; set; }
        public double Value { get; set; }
    }

    /// <summary>
    /// A page for showing Microsoft Band 2 sensors data including continuous time series graphs.
    /// This page also shows connected Empatica E4 serial number, current view in IOS wearable 
    /// application for E4, and current subject id of the subject running IOS wearable application
    /// </summary>
    public sealed partial class MSBandPage
    {
        private MSBandService Service { get; } = MSBandService.Singleton;
        private WebSocketService SocketService { get; } = WebSocketService.Singleton;
        private SubjectViewModel SubjectAndView { get; } = new SubjectViewModel();
        private ObservableCollection<string> AvailableBands { get; } = new ObservableCollection<string>();
        public ChartValues<MeasureModel> GsrDataPoint { get; } = new ChartValues<MeasureModel>();
        public ChartValues<MeasureModel> IbiDataPoint { get; } = new ChartValues<MeasureModel>();
        private DispatcherTimer Timer { get; set; }
        private double gsrValue;

        public MSBandPage()
        {
            InitializeComponent();
            Service.HeartRate.SensorValueChanged += HeartRateSensorValueChanged;
            Service.Gsr.SensorValueChanged += GsrSensorValueChanged;
            Service.RRInterval.SensorValueChanged += IbiSensorValueChanged;
            AddLiveCharts();
        }

        private void AddLiveCharts()
        {
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime)
                .Y(model => model.Value);

            Charting.For<MeasureModel>(mapper);

            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            Timer.Tick += TimerOnTick;
        }

        private async void TimerOnTick(object sender, object eventArgs)
        {

            await RunLaterInUIThread(() =>
            {
                GsrDataPoint.Add(new MeasureModel
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

        private async Task IbiSensorValueChanged(RRIntervalEvent value)
        {
            await RunLaterInUIThread(() =>
            {
                IbiDataPoint.Add(new MeasureModel
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

        private async Task GsrSensorValueChanged(GSREvent value)
        {
            await RunLaterInUIThread(() =>
            {
                gsrValue = value.Gsr;
            });
        }

        private async Task HeartRateSensorValueChanged(HeartRateEvent value)
        {
            await RunLaterInUIThread(() => heartRatePath.Fill = new SolidColorBrush(Locked == Service.HeartRate.HeartRateStatus ? White : Transparent)).ConfigureAwait(false);
        }

        private async void PageLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await SearchBands();
            //await SocketService.Connect(OnMessageReceived).ConfigureAwait(false);
        }

        private async void PageUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await Task.CompletedTask;
        }

        private async void SearchBandButtonAction(object sender, RoutedEventArgs e)
        {
            await LoadBands();
        }


        private async Task OnEmpaticaE4BandMessageReceived(EmpaticaE4Band empaticaE4Band)
        {
            SubjectViewService.Singleton.CurrentView = empaticaE4Band.FromView;
            SubjectViewService.Singleton.SubjectId = empaticaE4Band.SubjectId;
            
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


        private async Task LoadBands()
        {
            commandBar.Visibility = Visibility.Collapsed;
            await HideAllGrids("Loading Paired Bands", true);
            await Task.Delay(200);
            IEnumerable<string> availableBandNames = await Service.GetPairedBands();
            if (!availableBandNames.Any())
            {
                var messageDialog = new MessageDialog("")
                {
                    Content = "No Paired Bands Available!"
                };

                messageDialog.Commands.Add(new UICommand("Pair Your Band Now! Make Sure Your Band Is Paired In Bluetooth Mode", new UICommandInvokedHandler(CommandInvokedHandler), 1));
                messageDialog.Commands.Add(new UICommand("Not Now", new UICommandInvokedHandler(CommandInvokedHandler), -1));
                _ = await messageDialog.ShowAsync();
                await SearchBands();
            }
            else
            {
                await RunLaterInUIThread(() =>
                {
                    foreach (string device in availableBandNames)
                    {
                        AvailableBands.Add(device);
                    }

                    availableBandGrid.Visibility = Visibility.Visible;
                });
            }
        }

        private async void CommandInvokedHandler(IUICommand command)
        {
            switch ((int)command.Id)
            {
                case 1:
                    _ = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:bluetooth"));
                    break;
                default:
                    break;
            }
        }

        private async Task HideAllGrids(string message, bool isProgress = true)
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

        protected override async void OnNavigatedTo(NavigationEventArgs navigationEventArgs)
        {
            await Task.CompletedTask;
            base.OnNavigatedTo(navigationEventArgs);
            Debug.WriteLine("223");
        }

        private async void SyncBandButtonAction(object sender, RoutedEventArgs routedEventArgs)
        {
            await Task.CompletedTask;
        }

        private async void StartOrStopSessionButttonAction(object sender, RoutedEventArgs routedEventArgs)
        {
            var symbolIcon = new SymbolIcon(Symbol.Pause);
            string label = "Pause Session";
            bool sessionInProgress = true;

            if (SubjectViewService.Singleton.IsSessionInProgress)
            {
                symbolIcon = new SymbolIcon(Symbol.Play);
                label = "Resume Session";
                sessionInProgress = false;
            }

            SubjectViewService.Singleton.IsSessionInProgress = sessionInProgress;
            await RunLaterInUIThread(() =>
            {
                startOrStopSessionButtton.Icon = symbolIcon;
                startOrStopSessionButtton.Label = label;
            });
        }

        private async Task SearchBands()
        {
            await RunLaterInUIThread(() =>
            {
                searchBandGrid.Visibility = Visibility.Collapsed;
                availableBandGrid.Visibility = Visibility.Collapsed;
                searchBandGrid.Visibility = Visibility.Visible;
            });
        }

        private async void BandSelectionChanged(object sender, SelectionChangedEventArgs changedEventArgs)
        {
            await ConnectBand(availableBandComboBox.SelectedValue.ToString(), availableBandComboBox.SelectedIndex);
        }

        private async Task ConnectBand(string bandName, int selectedIndex)
        {
            var msgDlg = new MessageDialog(string.Empty);
            syncStackPanel.Visibility = Visibility.Visible;
            commandBar.IsEnabled = false;

            await HideAllGrids($"Connecting to band ({bandName})...");

            try
            {
                await Service.ConnectBand(bandName, selectedIndex);
            }
            catch (BandAccessDeniedException)
            {
                msgDlg.Content = $"Microsoft Band ({bandName}) doesn't have a permission to synchorize with this device";
                msgDlg.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(CommandInvokedHandler), -1));
            }
            catch (BandIOException)
            {
                msgDlg.Content = $"Failed to connect to Microsoft Band ({bandName}).";
                msgDlg.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(CommandInvokedHandler), -1));
            }
            catch (Exception ex)
            {
                msgDlg.Content = ex.ToString();
            }
            finally
            {
                if (!string.IsNullOrEmpty(msgDlg.Content))
                {
                    syncStackPanel.Visibility = Visibility.Collapsed;
                    _ = await msgDlg.ShowAsync();
                }
                else
                {
                    await StartDashboard();
                }
            }
        }

        private async Task StartDashboard()
        {
            await HideAllGrids($"Preparing Dashboard for Microsoft Band ({Service.BandName})...");
            await Service.SubscribeSensors();
            await RunLaterInUIThread(() =>
            {
                commandBar.Visibility = Visibility.Visible;
                SubjectAndView.MSBandSerialNumber = Service.BandName;
                UpdateCommandBar();
            });
            Timer.Start();

            await SocketService.Connect(OnEmpaticaE4BandMessageReceived).ConfigureAwait(false);
        }

        private void UpdateCommandBar()
        {
            switch (Service.BandStatus)
            {
                case BandStatus.Connected:
                    return;
                case BandStatus.Subscribed:
                    syncBandButton.Icon = new SymbolIcon(Symbol.DisableUpdates);
                    syncBandButton.Label = "Unsync Band";

                    syncGrid.Visibility = Visibility.Collapsed;
                    startOrStopSessionButtton.IsEnabled = true;

                    commandBar.IsEnabled = true;
                    commandBar.IsOpen = true;
                    break;

                case BandStatus.UnSubscribed:
                    syncBandButton.Icon = new SymbolIcon(Symbol.Sync);
                    syncBandButton.Label = "sync band";
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
