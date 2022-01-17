using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;
using System.Collections.Generic;
using Microsoft.Band.Sensors;
using static Microsoft.Band.Sensors.HeartRateQuality;
using static Windows.UI.Colors;
using System.Diagnostics;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Configurations;

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
        private SubjectViewModel SubjectAndView { get; } = new SubjectViewModel();
        private ObservableCollection<string> AvailableBands { get; } = new ObservableCollection<string>();

        public ChartValues<MeasureModel> GsrDataPoint { get; } = new ChartValues<MeasureModel>();
        public ChartValues<MeasureModel> IbiDataPoint { get; } = new ChartValues<MeasureModel>();

        public DispatcherTimer Timer { get; set; }

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
            CartesianMapper<MeasureModel> mapper = Mappers.Xy<MeasureModel>()
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
                gsrValue = 1 / value.Gsr;
            });
        }

        private async Task HeartRateSensorValueChanged(HeartRateEvent value)
        {
            await RunLaterInUIThread(() => heartRatePath.Fill = new SolidColorBrush(Locked == Service.HeartRate.HeartRateStatus ? White : Transparent)).ConfigureAwait(false);
        }

        private async void PageLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await SearchBands();
        }

        private async void PageUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await Task.CompletedTask;
        }

        private async void SearchBandButtonAction(object sender, RoutedEventArgs e)
        {
            await LoadBands();
        }

        private async Task LoadBands()
        {
            commandBar.Visibility = Visibility.Collapsed;
            await SetSyncMessage("Loading Paired Bands", true);
            await Task.Delay(200);
            IEnumerable<string> availableBandNames = await Service.GetPairedBands();
            if (!availableBandNames.Any())
            {
                MessageDialog messageDialog = new MessageDialog("")
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

        private async Task SetSyncMessage(string message, bool progress = true)
        {
            await RunLaterInUIThread(() =>
            {
                searchBandGrid.Visibility = Visibility.Collapsed;
                availableBandGrid.Visibility = Visibility.Collapsed;
                syncProgressRing.IsActive = progress;
                syncTextBlock.Text = message;
                syncGrid.Visibility = Visibility.Visible;
            });
        }

        protected override async void OnNavigatedTo(NavigationEventArgs navigationEventArgs)
        {
            await Task.CompletedTask;
            base.OnNavigatedTo(navigationEventArgs);
        }

        private async void SyncBandButtonAction(object sender, RoutedEventArgs routedEventArgs)
        {
            await Task.CompletedTask;
        }

        private async void StartOrStopSessionButttonAction(object sender, RoutedEventArgs routedEventArgs)
        {
            SymbolIcon symbolIcon = new SymbolIcon(Symbol.Pause);
            string label = "Pause Session";
            bool sessionInProgress = true;

            if (SubjectViewService.Singleton.IsSessionInProgress.Value)
            {
                symbolIcon = new SymbolIcon(Symbol.Play);
                label = "Resume Session";
                sessionInProgress = false;
            }

            SubjectViewService.Singleton.IsSessionInProgress.Value = sessionInProgress;
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
            MessageDialog msgDlg = new MessageDialog("");
            syncStackPanel.Visibility = Visibility.Visible;
            commandBar.IsEnabled = false;

            await SetSyncMessage("Connecting to your band...");

            try
            {
                await Service.ConnectBand(bandName, selectedIndex);
            }
            catch (BandAccessDeniedException)
            {
                msgDlg.Content = "Make sure your Microsoft Band (" + bandName + ") has permission to synchorize to this device.";
                msgDlg.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(CommandInvokedHandler), -1));
            }
            catch (BandIOException)
            {
                msgDlg.Content = "Failed to connect to your Microsoft Band (" + bandName + ").";
                msgDlg.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(CommandInvokedHandler), -1));
            }
            catch (Exception ex)
            {
                msgDlg.Content = ex.ToString();
            }
            finally
            {
                if (msgDlg.Content != "")
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
            await SetSyncMessage("Preparing the sensor log...");
            // Unsuscribe all sensors (current active suscriptions)
            //await band.UnsuscribeSensors();

            // Suscribe all sensors
            await Service.SubscribeSensors();

            //await StartBandClock();
            await RunLaterInUIThread(() =>
            {
                commandBar.Visibility = Visibility.Visible;
                SubjectAndView.MSBandSerialNumber = Service.BandName;
            });

            UpdateUI();
            Timer.Start();
        }

        private void UpdateUI()
        {
            switch (Service.BandStatus)
            {
                case BandStatus.SYNCED:
                case BandStatus.SYNCED_LIMITED_ACCESS:

                    syncBandButton.Icon = new SymbolIcon(Symbol.DisableUpdates);
                    syncBandButton.Label = "Unsync Band";

                    syncGrid.Visibility = Visibility.Collapsed;
                    startOrStopSessionButtton.IsEnabled = true;

                    commandBar.IsEnabled = true;
                    commandBar.IsOpen = true;
                    break;
                case BandStatus.SYNCED_TERMINATED:

                    syncBandButton.Icon = new SymbolIcon(Symbol.Sync);
                    syncBandButton.Label = "sync band";
                    startOrStopSessionButtton.IsEnabled = false;
                    break;
                default:
                    commandBar.IsEnabled = false;
                    break;
            }
        }
    }
}
