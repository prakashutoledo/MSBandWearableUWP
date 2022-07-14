using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using static System.Text.Encoding;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// Base test class for <see cref="JsonConverter{T}"/>
    /// </summary>
    /// <typeparam name="AnyConverter"></typeparam>
    /// <typeparam name="AnyType"></typeparam>
    public class BaseJsonConverterTest<AnyConverter, AnyType> where AnyConverter : JsonConverter<AnyType>, new()
    {
        protected AnyConverter jsonConverter;

        [TestInitialize]
        public void ConverterSetup()
        {
            jsonConverter = new AnyConverter();
        }

        [TestMethod]
        public void ShouldNotConvertToUnknownType()
        {
            Assert.IsFalse(jsonConverter.CanConvert(typeof(string)), "string type is not convertible");
            Assert.IsTrue(jsonConverter.CanConvert(typeof(AnyType)), "Converter enum is convertible");
        }

        [TestMethod]
        public void ShouldFailedReadForNonConvertibleTypeAndNone()
        {
            var exception = Assert.ThrowsException<ArgumentException>(InvalidConverterRead(typeof(string)));
            Assert.AreEqual($"String is not convertible to {typeof(AnyType).Name}", exception.Message);

            var exception1 = Assert.ThrowsException<InvalidOperationException>(InvalidConverterRead(typeof(AnyType)));
            Assert.AreEqual("No value has been read by reader", exception1.Message);

            var exception2 = Assert.ThrowsException<NullReferenceException>(InvalidConverterRead(typeof(AnyType), "null", true));
            Assert.AreEqual($"Cannot convert `null` to {typeof(AnyType).Name}", exception2.Message);
        }

        /// <summary>
        /// Verify the read operation has been successfully performed for this json converter
        /// </summary>
        /// <param name="description">A string description to read</param>
        /// <param name="expected">An expected read value</param>
        /// <param name="message">A massege to set</param>
        protected void VerifyRead(string description, AnyType expected, string message)
        {
            Assert.AreEqual(expected, Read(typeof(AnyType), description, true), message);
        }

        /// <summary>
        /// Verifies this json converter can write the value
        /// </summary>
        /// <param name="anyType">A value to write</param>
        /// <param name="expected">An expected string value represented for input value</param>
        /// <param name="message">A message to set</param>
        /// <returns>A task that can be awaited</returns>
        protected async Task VerifyWrite(AnyType anyType, string expected, string message)
        {
            using (Stream stream = new MemoryStream())
            using (var jsonWriter = new Utf8JsonWriter(stream, default))
            using (var stringReader = new StreamReader(stream, UTF8))
            {
                jsonConverter.Write(jsonWriter, anyType, default);
                await jsonWriter.FlushAsync();

                stream.Seek(0, SeekOrigin.Begin);
                var actual = await stringReader.ReadToEndAsync();

                Assert.AreEqual(expected, actual, message);
            }
        }

        protected Action InvalidConverterRead(Type toType, string value = null, bool canRead = false)
        {
            return () => Read(toType, value, canRead);
        }

        protected AnyType Read(Type toType, string value = null, bool canRead = false)
        {
            var bytes = value == null ? null : new ReadOnlySpan<byte>(UTF8.GetBytes(value));
            var jsonReader = new Utf8JsonReader(bytes, default);
            if (canRead)
            {
                jsonReader.Read();
            }
            return jsonConverter.Read(ref jsonReader, toType, default);
        }
    }
}
