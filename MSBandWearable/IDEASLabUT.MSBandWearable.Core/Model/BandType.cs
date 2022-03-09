using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    public enum BandType
    {
        E4Band,
        MSBand
    }

    public static class BandTypeExtension
    {
        public static string GetDescription(this BandType payloadType)
        {
            switch (payloadType)
            {
                case BandType.E4Band:
                    return "E4BAND";
                case BandType.MSBand:
                    return "MSBAND";
                default:
                    return null;
            }
        }

        public static BandType? FromDescription(string description)
        {
            if (description == null)
            {
                return null;
            }

            switch (description)
            {
                case "E4BAND":
                    return BandType.E4Band;
                case "MSBAND":
                    return BandType.MSBand;
                default:
                    return null;
            }
        }
    }
}
