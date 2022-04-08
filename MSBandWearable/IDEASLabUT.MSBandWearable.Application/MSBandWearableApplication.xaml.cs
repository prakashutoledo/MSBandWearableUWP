﻿using IDEASLabUT.MSBandWearable.Application.Views;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

/// <summary>
/// <auther>Prakash Khadka</auther>
/// <mailto>prakashkhadka@aol.com</mailto>
/// </summary>
namespace IDEASLabUT.MSBandWearable.Application
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
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
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
                Window.Current.Activate();
            }
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