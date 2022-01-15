using IDEASLabUT.MSBandWearable.Application.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IDEASLabUT.MSBandWearable.Application
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            AccelerometerEvent sensor = new AccelerometerEvent
            {
                AccelerationX = 1.4,
                AccelerationY = 1.5,
                AccelerationZ = 2.6,
                AcquiredTime = DateTime.Now,
                ActualTime = DateTime.Now,
                FromView = "Test View",
                SubjectId = "Test Subject"
            };
            Debug.WriteLine(sensor.ToString());
        }
    }
}
