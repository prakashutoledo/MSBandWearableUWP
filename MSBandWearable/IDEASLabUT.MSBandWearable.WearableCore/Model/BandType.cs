/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

using static IDEASLabUT.MSBandWearable.Model.BandType;
using static IDEASLabUT.MSBandWearable.WearableCoreGlobals;

namespace IDEASLabUT.MSBandWearable.Model
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
        private static readonly Lazy<IReadOnlyDictionary<string, BandType>> bandTypeMap;
        private static readonly Lazy<IReadOnlyDictionary<BandType, string>> descriptionMap;

        static BandTypeExtension()
        {
            bandTypeMap = new Lazy<IReadOnlyDictionary<string, BandType>>(() =>
            {
                return new Dictionary<string, BandType>()
                {
                    { E4BandDescription, E4Band },
                    { MSBandDescription, MSBand }
                };
            });

          
            descriptionMap = new Lazy<IReadOnlyDictionary<BandType, string>>(() => BandTypeMap.ToDictionary(bandTypeEntry => bandTypeEntry.Value, bandTypeEntry => bandTypeEntry.Key));
        }

        private static IReadOnlyDictionary<string, BandType> BandTypeMap => bandTypeMap.Value;
        private static IReadOnlyDictionary<BandType, string> DescriptionMap => descriptionMap.Value;

        /// <summary>
        /// Gets the description of represented band type
        /// </summary>
        /// <param name="bandType">A band type enum value</param>
        /// <returns>A string representation of this band type</returns>
        public static string GetDescription(this BandType bandType)
        {
            return DescriptionMap[bandType];
        }

        /// <summary>
        /// Gets the matching band type enum value for the given description
        /// </summary>
        /// <param name="description">A description of enum value to match</param>
        /// <returns>A matching nullable <see cref="BandType?"/></returns>
        public static BandType? ToBandType(this string description)
        {
            return description == null ? null : BandTypeMap.TryGetValue(description, out var bandType) ? (BandType?) bandType : null;
        }
    }
}
