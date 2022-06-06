using static IDEASLabUT.MSBandWearable.Util.MSBandWearableCoreUtil;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// An enum representing websocket message payload type
    /// </summary>
    public enum PayloadType
    {
        E4Band
    }

    /// <summary>
    /// An extension class for <see cref="PayloadType"/> to include description of the represented enum value
    /// </summary>
    public static class PayloadTypeExtension
    {
        /// <summary>
        /// Gets the description of representated payload type
        /// </summary>
        /// <param name="payloadType">A payload type enum</param>
        /// <returns>A string representation of this payload type</returns>
        public static string GetDescription(this PayloadType payloadType)
        {
            switch (payloadType)
            {
                case PayloadType.E4Band:
                    return E4BandPayloadTypeDescription;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the matching payload type enum value for the given description
        /// </summary>
        /// <param name="description">A description of enum value to match</param>
        /// <returns>A matching nullable <see cref="PayloadType?"/></returns>
        public static PayloadType? FromDescription(string description)
        {
            if (description == null)
            {
                return null;
            }

            switch (description)
            {
                case E4BandPayloadTypeDescription:
                    return PayloadType.E4Band;
                default:
                    return null;
            }
        }
    }
}
