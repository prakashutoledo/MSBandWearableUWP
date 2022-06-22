using GuerrillaNtp;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Model
{
    public interface INtpClientSupplier

    {
        Task<NtpClient> Supply(string ntpPool);
    }
}
