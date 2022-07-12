using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using static System.Text.Encoding;

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
        public void ShouldFailedReadForNullAndInvalidName()
        {
            var exception2 = Assert.ThrowsException<NullReferenceException>(() =>
            {
                var jsonReader = new Utf8JsonReader(new ReadOnlySpan<byte>(UTF8.GetBytes("null")));
                jsonReader.Read();
                _ = jsonConverter.Read(ref jsonReader, typeof(ConverterEnum), default);
            });

            Assert.AreEqual($"Cannot convert `null` to {typeof(ConverterEnum).Name}", exception2.Message);

            var exception3 = Assert.ThrowsException<NullReferenceException>(() =>
            {
                var jsonReader = new Utf8JsonReader(new ReadOnlySpan<byte>(UTF8.GetBytes("\"some\"")));
                jsonReader.Read();
                _ = jsonConverter.Read(ref jsonReader, typeof(ConverterEnum), default);
            });

            Assert.AreEqual($"Cannot convert `some` to {typeof(ConverterEnum).Name}", exception3.Message);
        }
    }
}
