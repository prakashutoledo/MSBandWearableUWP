/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

using static IDEASLabUT.MSBandWearable.Model.Notification.PayloadAction;
using static IDEASLabUT.MSBandWearable.NotificationGlobals;

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
        private static readonly Lazy<IReadOnlyDictionary<string, PayloadAction>> payloadActionMap;
        private static readonly Lazy<IReadOnlyDictionary<PayloadAction, string>> descriptionMap;

        static PayloadActionExtension()
        {
            payloadActionMap = new Lazy<IReadOnlyDictionary<string, PayloadAction>>(() =>
            {
                return new Dictionary<string, PayloadAction>()
                {
                    { SendMessageDescription, SendMessage }
                };
            });

            descriptionMap = new Lazy<IReadOnlyDictionary<PayloadAction, string>>(() => PayloadActionMap.ToDictionary(entry => entry.Value, entry => entry.Key));
        }

        /// <summary>
        /// A readonly payload description by payload action
        /// </summary>
        private static IReadOnlyDictionary<string, PayloadAction> PayloadActionMap => payloadActionMap.Value;

        /// <summary>
        /// A readonly payload action by description map
        /// </summary>
        private static IReadOnlyDictionary<PayloadAction, string> DescriptionMap => descriptionMap.Value;

        /// <summary>
        /// Gets the description of represented payload action
        /// </summary>
        /// <param name="payloadAction">A payload type enum value</param>
        /// <returns>A string representation of this payload action</returns>
        public static string GetDescription(this PayloadAction payloadAction)
        {
            return DescriptionMap[payloadAction];
        }

        /// <summary>
        /// Gets the matching payload action enum value for the given description
        /// </summary>
        /// <param name="description">A description of enum value to match</param>
        /// <returns>A matching nullable <see cref="PayloadAction?"/></returns>
        public static PayloadAction? ToPayloadAction(this string description)
        {
            return description == null ? null : PayloadActionMap.TryGetValue(description, out PayloadAction payloadAction) ? (PayloadAction?) payloadAction : null;
        }
    }
}
