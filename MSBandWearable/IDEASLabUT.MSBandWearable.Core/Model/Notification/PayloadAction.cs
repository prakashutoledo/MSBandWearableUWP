using static IDEASLabUT.MSBandWearable.Util.MSBandWearableCoreUtil;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// An enum representating webSocket message payload action
    /// </summary>
    public enum PayloadAction
    {
        SendMessage
    }

    /// <summary>
    /// An extension class for <see cref="PayloadAction"/> to include description of the represented enum value
    /// </summary>
    public static class PayloadActionExtension
    {
        /// <summary>
        /// Gets the description of represented payload action
        /// </summary>
        /// <param name="payloadAction">A payload type enum value</param>
        /// <returns>A string representation of this payload action</returns>
        public static string GetDescription(this PayloadAction payloadAction)
        {
            switch (payloadAction)
            {
                case PayloadAction.SendMessage:
                    return SendMessageDescription;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the matching payload action enum value for the given description
        /// </summary>
        /// <param name="description">A description of enum value to match</param>
        /// <returns>A matching nullable <see cref="PayloadAction?"/></returns>
        public static PayloadAction? FromDescription(string description)
        {
            if (description == null)
            {
                return null;
            }

            switch (description)
            {
                case SendMessageDescription:
                    return PayloadAction.SendMessage;
                default:
                    return null;
            }
        }
    }
}
