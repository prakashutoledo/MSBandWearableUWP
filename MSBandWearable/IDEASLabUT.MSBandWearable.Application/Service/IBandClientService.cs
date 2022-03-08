using Microsoft.Band;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
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
        /// <returns></returns>
        Task ConnectBand(int selectedIndex);
    }
}
