using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using IDEASLabUT.MSBandWearable.Application.ViewModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IDEASLabUT.MSBandWearable.Application.Views
{
    /// <summary>
    /// A page for showing Microsoft Band 2 sensors data including continuous time series graphs.
    /// This page also shows connected Empatica E4 serial number, current view in IOS wearable 
    /// application for E4, and current subject id of the subject running IOS wearable application
    /// </summary>
    public sealed partial class MSBandPage : Page
    {
        private MSBandService Service { get; } = MSBandService.Singleton;
        private SubjectViewModel SubjectAndView { get; } = new SubjectViewModel();

        public MSBandPage()
        {
            InitializeComponent();
            Service.Accelerometer.SensorValueChanged += AccelerometerSensorValueChanged;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs navigationEventArgs)
        {
            await Service.ConnectBand();
            await Service.SubscribeSensors();
            base.OnNavigatedTo(navigationEventArgs);
        }

        public async Task AccelerometerSensorValueChanged(AccelerometerEvent accelerometerEvent)
        {
            Debug.WriteLine(accelerometerEvent.ToString());
            await Task.CompletedTask;
        }
    }
}
