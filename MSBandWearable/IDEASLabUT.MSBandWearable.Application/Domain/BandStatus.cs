using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Domain
{
    public enum BandStatus
    {
        NO_PAIR_BAND_FOUND,
        NO_SYNC_PERMISSION,
        BAND_IO_EXCEPTION,
        SYNC_ERROR,
        SYNCED,
        SYNCED_SUSCRIBING,
        SYNCED_LIMITED_ACCESS,
        SYNCED_TERMINATED,
        UNKNOWN
    }
}
