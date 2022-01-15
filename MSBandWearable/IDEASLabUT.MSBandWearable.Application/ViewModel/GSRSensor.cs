using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    public class GSRSensor : BaseSensorModel<GSREvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;

        public GSRSensor() : base(new GSREvent())
        {
        }

        private double Gsr
        {
            set
            {
                Model.Gsr = value;
                NotifyPropertyChanged(nameof(Model));
            }
        }

        /// <summary>
        /// A task that can subscribe GSR sensor from Microsoft Band 2
        /// </summary>
        /// <returns>An object used to await this task</returns>
        public override async Task Subscribe()
        {
            await base.Subscribe().ConfigureAwait(false);
            var gsr = MSBandService.Singleton.BandClient.SensorManager.Gsr;
            gsr.ReadingChanged += GsrReadingChanged;
            _ = await gsr.StartReadingsAsync().ConfigureAwait(false);
        }

        private async void GsrReadingChanged(object sender, BandSensorReadingEventArgs<IBandGsrReading> readingEventArgs)
        {
            var subjectViewService = SubjectViewService.Singleton;
            var gsrReading = readingEventArgs.SensorReading;
            var gsrEvent = new GSREvent
            {
                Gsr = gsrReading.Resistance,
                AcquiredTime = DateTime.Now,
                ActualTime = gsrReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView.Value,
                SubjectId = subjectViewService.SubjectId.Value
            };

            await RunLaterInUIThread(() => Gsr = gsrEvent.Gsr);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(gsrEvent);
            }
        }
    }
}