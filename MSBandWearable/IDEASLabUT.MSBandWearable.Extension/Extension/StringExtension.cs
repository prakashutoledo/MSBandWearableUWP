﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
namespace IDEASLabUT.MSBandWearable.Extension
{
    /// <summary>
    /// A utility class for string manipulation extension methods
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Remove nth character from given index from last
        /// </summary>
        /// <param name="value">A string value to remove character from</param>
        /// <param name="indexFromLast">An index value from last</param>
        /// <returns>A string with removed nth character from last if valid otherwise returns itself</returns>
        /// <remarks>Index from last is 1 based not zero based</remarks>
        public static string RemoveNthCharacterFromLast(this string value, in int indexFromLast)
        {
            return indexFromLast <= 0 || indexFromLast > value.Length ? value : value.Remove(value.Length - indexFromLast, 1);
        }
    }
}
