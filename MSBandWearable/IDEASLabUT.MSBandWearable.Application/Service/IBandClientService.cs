using Microsoft.Band;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public interface IBandClientService
    {
        IBandClient BandClient { get; set; }
        Task ConnectBand(int selectedIndex);
    }
}
