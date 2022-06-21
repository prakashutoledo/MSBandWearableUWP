using IDEASLabUT.MSBandWearable.Model;

using Microsoft.Band.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

using static Microsoft.Band.Sensors.HeartRateQuality;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// View model test for <see cref="HeartRateSensor"/>
    /// </summary>
    [TestClass]
    public class HeartRateSensorTest : BaseSensorTest<HeartRateEvent, IBandHeartRateReading, HeartRateSensor>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeartRateSensorTest"/>
        /// </summary>
        public HeartRateSensorTest() : base(sensorManager => sensorManager.HeartRate)
        {
        }

        [TestMethod]
        public async Task OnHeartRateReadingChanged()
        {
            await MockSensorReadingChanged(When(reading => reading.HeartRate, 97), When(reading => reading.Quality, Locked));

            var expectedModel = NewModel(value => value.Bpm = 97);
            VerifySensorValueChanged(expectedModel: expectedModel, extraPropertyVerifier: viewModel =>
            {
                // Min and max bpm should be 97.0 for first heart rate reading event
                VerifyHeartRate(expectedHeartStatus: (Locked, 1), expectedMax: (97, 1), expectedMin: (97, 1), viewModel: viewModel);
            });

            await MockSensorReadingChanged(When(reading => reading.HeartRate, 110), When(reading => reading.Quality, Locked));

            expectedModel = NewModel(value => value.Bpm = 110);
            VerifySensorValueChanged(expectedModel: expectedModel, expectedCount: 2, extraPropertyVerifier: viewModel =>
            {
                // Max bpm should be 110 but min bpm should remain unchanged
                VerifyHeartRate(expectedHeartStatus: (Locked, 1), expectedMax: (110, 2), expectedMin: (97, 1), viewModel: viewModel);
            });

            await MockSensorReadingChanged(When(reading => reading.HeartRate, 92), When(reading => reading.Quality, Acquiring));

            expectedModel = NewModel(value => value.Bpm = 92);
            VerifySensorValueChanged(expectedModel: expectedModel, expectedCount: 3, extraPropertyVerifier: viewModel =>
            {
                // Min bom should be 92.0 but max bpm should be the previous value
                VerifyHeartRate(expectedHeartStatus: (Acquiring, 2), expectedMax: (110, 2), expectedMin: (92, 2), viewModel: viewModel);
            });
        }

        private void VerifyHeartRate((HeartRateQuality quality, int count) expectedHeartStatus, (double bpm, int count) expectedMax, (double bpm, int count) expectedMin, HeartRateSensor viewModel)
        {
            VerifyProperty("HeartRateStatus", expectedHeartStatus.count);
            VerifyProperty("MaxBpm", expectedMax.count);
            VerifyProperty("MinBpm", expectedMin.count);
        }
    }
}
