using static IDEASLabUT.MSBandWearable.Core.Util.MSBandWearableCoreUtil;

namespace IDEASLabUT.MSBandWearable.Core.Model
{
    /// <summary>
    /// An enum representing wearable band types
    /// </summary>
    public enum BandType
    {
        E4Band, // Empatica E4 Band
        MSBand  // Microsoft Band 2
    }

    /// <summary>
    /// An extension class for <see cref="BandType"/> to include description of the represented enum value
    /// </summary>
    public static class BandTypeExtension
    {
        /// <summary>
        /// Gets the description of represented band type
        /// </summary>
        /// <param name="payloadType">A band type enum value</param>
        /// <returns>A string representation of this band type</returns>
        public static string GetDescription(this BandType payloadType)
        {
            switch (payloadType)
            {
                case BandType.E4Band:
                    return E4BandDescription;
                case BandType.MSBand:
                    return MSBandDescription;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the matching band type enum value for the given description
        /// </summary>
        /// <param name="description">A description of enum value to match</param>
        /// <returns>A matching nullable <see cref="BandType?"/></returns>
        public static BandType? FromDescription(string description)
        {
            if (description == null)
            {
                return null;
            }

            switch (description)
            {
                case E4BandDescription:
                    return BandType.E4Band;
                case MSBandDescription:
                    return BandType.MSBand;
                default:
                    return null;
            }
        }
    }
}
