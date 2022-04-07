namespace IDEASLabUT.MSBandWearable.Core.Model.Notification
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
        /// Gets the description of representation payload action
        /// </summary>
        /// <param name="payloadAction">A payload action enum</param>
        /// <returns>A string representation of this payload action</returns>
        public static string GetDescription(this PayloadAction payloadAction)
        {
            switch (payloadAction)
            {
                case PayloadAction.SendMessage:
                    return "sendMessage";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the matching enum value for the given description
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
                case "sendMessage":
                    return PayloadAction.SendMessage;
                default:
                    return null;
            }
        }
    }
}
