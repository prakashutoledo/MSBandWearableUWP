using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.Util
{
    [TestClass]
    public class StringUtilTest
    {
        [DataTestMethod]
        [DataRow("abcde", "abcde", 7)]
        [DataRow("abcde", "bcde", 5)]
        [DataRow("abcde", "abcd", 1)]
        [DataRow("abcde", "abcde", 0)]
        [DataRow("abcde", "abcde", -1)]
        public void RemoveNthCharacterFromString(string actual, string expected, int indexFromLast)
        {
            Assert.AreEqual(expected, actual.RemoveNthCharacterFromLast(indexFromLast));
        }
    }
}
