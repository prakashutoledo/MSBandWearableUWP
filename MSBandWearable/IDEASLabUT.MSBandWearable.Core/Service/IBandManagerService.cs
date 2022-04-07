using IDEASLabUT.MSBandWearable.Core.Model;
using IDEASLabUT.MSBandWearable.Core.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Core.Service
{
    /// <summary>
    /// An interface for managing supported sensors, see the status of conneted MS Band 2
    /// </summary>
    public interface IBandManagerService
    {
        /// <summary>
        /// The current status of the connected MS Band 2
        /// </summary>
        BandStatus BandStatus { get; set; }

        /// <summary>
        /// The unique name of the connected MS Band 2
        /// </summary>
        string BandName { get; set; }

        /// <summary>
        /// An accelerometer sensor of the connected MS Band 2
        /// </summary>
        AccelerometerSensor Accelerometer { get; set; }

        /// <summary>
        /// A gsr sensor of the connected MS Band 2
        /// </summary>
        GSRSensor Gsr { get; set; }

        /// <summary>
        /// A gyroscope sensor of the connected MS Band 2
        /// </summary>
        GyroscopeSensor Gyroscope { get; set; }

        /// <summary>
        /// A heart rate sensor of the connected MS Band 2
        /// </summary>
        HeartRateSensor HeartRate { get; set; }

        /// <summary>
        /// A temperature sensor of the connected MS Band 2
        /// </summary>
        TemperatureSensor Temperature { get; set; }

        /// <summary>
        /// A rr interval sensor of the connected MS Band 2
        /// </summary>
        RRIntervalSensor RRInterval { get; set; }

        /// Connects the given selected index from the available paired MS bands with given name
        /// </summary>
        /// <param name="selectedIndex">A selected index of a paired bands</param>
        /// <param name="bandName">A name of the band to connect</param>
        /// <returns>A task that can be awaited</returns>
        Task ConnectBand(int selectedIndex, string bandName);

        /// <summary>
        /// Subscribe all available sensors of connected MS Band 2
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        Task SubscribeSensors();

        /// <summary>
        /// Find all the MS Band 2 which are paired using bluetooth
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        Task<IEnumerable<string>> GetPairedBands();
    }
}
