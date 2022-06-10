namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// A utility class for string manipulation
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// Remove nth character from given index from last
        /// </summary>
        /// <param name="value">A string value to remove character from</param>
        /// <param name="indexFromLast">An index value from last</param>
        /// <returns>A string with removed nth character from last if valid otherwise returns itself</returns>
        /// <remarks>Index from last is 1 based not zero based</remarks>
        public static string RemoveNthCharacterFromLast(this string value, int indexFromLast)
        {
            return indexFromLast <= 0 || indexFromLast > value.Length ? value : value.Remove(value.Length - indexFromLast, 1);
        }
    }
}
