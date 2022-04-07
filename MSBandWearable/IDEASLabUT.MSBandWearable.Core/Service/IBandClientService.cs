using Microsoft.Band;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Core.Service
{
    /// <summary>
    /// Interface for MS Band 2 client service to connect to band and subscribe available supported sensors
    /// </summary>
    public interface IBandClientService
    {
        /// <summary>
        /// A connected MS Band 2 client 
        /// </summary>
        IBandClient BandClient { get; set; }

        /// <summary>
        /// Connects the given selected index from the available paired MS bands
        /// </summary>
        /// <param name="selectedIndex">A selected index of a paired bands</param>
        /// <returns>A task that can be awaited</returns>
        Task ConnectBand(int selectedIndex);
    }
}
