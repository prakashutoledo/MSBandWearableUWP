using System;
using System.Collections.Generic;
using System.Linq;

using static IDEASLabUT.MSBandWearable.Model.Notification.PayloadType;
using static IDEASLabUT.MSBandWearable.NotificationGlobals;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// An enum representing websocket message payload type
    /// </summary>
    public enum PayloadType
    {
        E4Band,
        MSBand
    }

    /// <summary>
    /// An extension class for <see cref="PayloadType"/> to include description of the represented enum value
    /// </summary>
    public static class PayloadTypeExtension
    {
        private static readonly Lazy<IReadOnlyDictionary<string, PayloadType>> payloadTypeMap;
        private static readonly Lazy<IReadOnlyDictionary<PayloadType, string>> descriptionMap;

        static PayloadTypeExtension()
        {
            payloadTypeMap = new Lazy<IReadOnlyDictionary<string, PayloadType>>(() =>
            {
                return new Dictionary<string, PayloadType>()
                {
                    { E4BandPayloadTypeDescription, E4Band },
                    { MSBandPayloadTypeDescription, MSBand }
                };
            });

            descriptionMap = new Lazy<IReadOnlyDictionary<PayloadType, string>>(() => PayloadTypeMap.ToDictionary(entry => entry.Value, entry => entry.Key));
        }

        private static IReadOnlyDictionary<string, PayloadType> PayloadTypeMap => payloadTypeMap.Value;
        private static IReadOnlyDictionary<PayloadType, string> DescriptionMap => descriptionMap.Value;

        /// <summary>
        /// Gets the description of representated payload type
        /// </summary>
        /// <param name="payloadType">A payload type enum</param>
        /// <returns>A string representation of this payload type</returns>
        public static string GetDescription(this PayloadType payloadType)
        {
            return DescriptionMap[payloadType];
        }

        /// <summary>
        /// Gets the matching payload type enum value for the given description
        /// </summary>
        /// <param name="description">A description of enum value to match</param>
        /// <returns>A matching nullable <see cref="PayloadType?"/></returns>
        public static PayloadType? FromDescription(string description)
        {
            return description == null ? null : PayloadTypeMap.TryGetValue(description, out PayloadType payloadType) ? (PayloadType?) payloadType : null;
        }
    }
}
