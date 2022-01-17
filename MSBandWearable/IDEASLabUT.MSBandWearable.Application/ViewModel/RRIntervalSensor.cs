using static IDEASLabUT.MSBandWearable.Application.Util.MSBandWearableUtil;

using IDEASLabUT.MSBandWearable.Application.Model;
using IDEASLabUT.MSBandWearable.Application.Service;
using Microsoft.Band.Sensors;
using System;
using System.Threading.Tasks;
using Microsoft.Band;

namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    public class RRIntervalSensor : BaseSensorModel<RRIntervalEvent>
    {
        public event SensorValueChangedHandler SensorValueChanged;

        public RRIntervalSensor() : base(new RRIntervalEvent())
        {
        }

        private double Ibi
        {
            set
            {
                Model.Ibi = value;
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
            IBandSensor<IBandRRIntervalReading> ibi = MSBandService.Singleton.BandClient.SensorManager.RRInterval;

            bool userConsent = UserConsent.Granted == ibi.GetCurrentUserConsent() || await ibi.RequestUserConsentAsync().ConfigureAwait(false);
            if (!userConsent)
            {
                return;
            }

            ibi.ReadingChanged += RRIntervalReadingChanged;
            _ = await ibi.StartReadingsAsync().ConfigureAwait(false);
        }

        private async void RRIntervalReadingChanged(object sender, BandSensorReadingEventArgs<IBandRRIntervalReading> readingEventArgs)
        {
            SubjectViewService subjectViewService = SubjectViewService.Singleton;
            IBandRRIntervalReading rrIntervalReading = readingEventArgs.SensorReading;
            RRIntervalEvent ibiEvent = new RRIntervalEvent
            {
                Ibi = rrIntervalReading.Interval,
                AcquiredTime = NtpSyncService.Singleton.LocalTimeNow,
                ActualTime = rrIntervalReading.Timestamp.DateTime,
                FromView = subjectViewService.CurrentView,
                SubjectId = subjectViewService.SubjectId
            };

            await RunLaterInUIThread(() => Ibi = ibiEvent.Ibi).ConfigureAwait(false);

            if (SensorValueChanged != null)
            {
                await SensorValueChanged.Invoke(ibiEvent).ConfigureAwait(false);
            }


            if (SubjectViewService.Singleton.IsSessionInProgress)
            {

            }
        }
    }
}
