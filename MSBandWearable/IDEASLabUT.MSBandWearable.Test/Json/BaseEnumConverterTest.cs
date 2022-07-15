/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Text.Json.Serialization;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// Base test class for all class inheriting <see cref="BaseEnumConverterTest{Converter, ConverterEnum}"/>
    /// </summary>
    /// <typeparam name="Converter">A type of json converter</typeparam>
    /// <typeparam name="ConverterEnum">A type of enum</typeparam>
    public class BaseEnumConverterTest<Converter, ConverterEnum> : BaseJsonConverterTest<Converter, ConverterEnum> where Converter : JsonConverter<ConverterEnum>, new() where ConverterEnum : Enum
    {
        [TestMethod]
        public void ShouldFailedReadInvalidBandTypeDescription()
        {
            var exception = Assert.ThrowsException<NullReferenceException>(InvalidConverterRead(typeof(ConverterEnum), "\"some\"", true));
            Assert.AreEqual($"Cannot convert `some` to {typeof(ConverterEnum).Name}", exception.Message);
        }
    }
}
