using IDEASLabUT.MSBandWearable.Views;

using Serilog;

using System;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

/// <summary>
/// <author>Prakash Khadka</author>
/// <mailto>prakashkhadka@aol.com</mailto>
/// </summary>
namespace IDEASLabUT.MSBandWearable
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class MSBandWearableApplication : Windows.UI.Xaml.Application
    {
        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public MSBandWearableApplication()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="eventArgs">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs eventArgs)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }

            if (eventArgs.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    _ = rootFrame.Navigate(typeof(MSBandPage), eventArgs.Arguments);
                }
                AddApplicationCloseRequestHandler();
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Add callback handler for current page close request event
        /// </summary>
        private void AddApplicationCloseRequestHandler()
        {
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnApplicationCloseRequest;
        }

        /// <summary>
        /// A callback when user close the currently loaded page from UI
        /// </summary>
        /// <param name="sender">The sender of the current page close event</param>
        /// <param name="closeRequestEventArgs">A system navigation close request preview event arguments</param>
        private async void OnApplicationCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs closeRequestEventArgs)
        {
            var deferral = closeRequestEventArgs.GetDeferral();
            var messageDialog = new MessageDialog("Are you sure you want to exit?");
            var cancelCommand = new UICommand("Cancel");
            var closeCommand = new UICommand("Close");

            var commands = messageDialog.Commands;
            commands.Add(closeCommand);
            commands.Add(cancelCommand);

            var response = await messageDialog.ShowAsync();

            if (response == closeCommand)
            {
                var loggerTaskSource = new TaskCompletionSource<object>();
                await Task.Run(() =>
                {
                    // Sets the global logger
                    // On application close request, flush the logger and close it
                    //Log.Logger = Ser;
                    //Log.CloseAndFlush();
                    //loggerTaskSource.SetResult(null);
                });

                _ = await loggerTaskSource.Task;
            }
            else
            {
                closeRequestEventArgs.Handled = true;
            }
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="failedEventArgs">Details about the navigation failure</param>
        private async void OnNavigationFailed(object sender, NavigationFailedEventArgs failedEventArgs)
        {
            var messageDialog = new MessageDialog($"Failed to load Page {failedEventArgs.SourcePageType.FullName}", "Navigation Failed");
            messageDialog.Commands.Add(new UICommand("Close"));
            _ = await messageDialog.ShowAsync();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
