﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.Extension
{
    /// <summary>
    /// Unit test for <see cref="StringExtension"/>
    /// </summary>
    [TestClass]
    public class StringExtensionTest
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
