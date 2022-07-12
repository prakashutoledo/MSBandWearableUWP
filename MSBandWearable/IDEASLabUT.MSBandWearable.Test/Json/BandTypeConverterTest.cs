using IDEASLabUT.MSBandWearable.Model;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using static System.Text.Encoding;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// Unit test for <see cref="BandTypeConverter"/>
    /// </summary>
    [TestClass]
    public class BandTypeConverterTest
    {
        private BandTypeConverter bandTypeConverter;

        [TestInitialize]
        public void Setup()
        {
            bandTypeConverter = new BandTypeConverter();
        }

        [TestMethod]
        public void ShouldCanConvert()
        {
            Assert.IsFalse(bandTypeConverter.CanConvert(typeof(string)), "string type is not convertible");
            Assert.IsTrue(bandTypeConverter.CanConvert(typeof(BandType)), "string type is not convertible");
        }

        [TestMethod]
        public void ShouldFailedRead()
        {
            var exception = Assert.ThrowsException<ArgumentException>(() =>
            {
                var jsonReader = new Utf8JsonReader();
                bandTypeConverter.Read(ref jsonReader, typeof(string), default);
            });

            Assert.AreEqual("String is not convertible to BandType", exception.Message);

            var exception1 = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var jsonReader = new Utf8JsonReader();
                bandTypeConverter.Read(ref jsonReader, typeof(BandType), default);
            });
            

            Assert.AreEqual("No value is read by reader", exception1.Message);

            var exception2 = Assert.ThrowsException<NullReferenceException>(() =>
            {
                var jsonReader = new Utf8JsonReader(new ReadOnlySpan<byte>(UTF8.GetBytes("null")));
                jsonReader.Read();
                bandTypeConverter.Read(ref jsonReader, typeof(BandType), default);
            });

            Assert.AreEqual("Cannot convert `null` to BandType", exception2.Message);

            var exception3 = Assert.ThrowsException<NullReferenceException>(() =>
            {
                var jsonReader = new Utf8JsonReader(new ReadOnlySpan<byte>(UTF8.GetBytes("\"some\"")));
                jsonReader.Read();
                bandTypeConverter.Read(ref jsonReader, typeof(BandType), default);
            });

            Assert.AreEqual("Cannot convert `some` to BandType", exception3.Message);
        }

        [DataTestMethod]
        [DataRow("\"MSBAND\"", BandType.MSBand)]
        [DataRow("\"E4BAND\"", BandType.E4Band)]
        public void ShouldReadBandType(string description, BandType expectedBandType)
        {
            var jsonReader = new Utf8JsonReader(new ReadOnlySpan<byte>(UTF8.GetBytes(description)));
            jsonReader.Read();

            var actualBandType = bandTypeConverter.Read(ref jsonReader, typeof(BandType), default);

            Assert.AreEqual(expectedBandType, actualBandType, "Band type should match");
        }

        [DataTestMethod]
        [DataRow(BandType.MSBand, "\"MSBAND\"")]
        [DataRow(BandType.E4Band, "\"E4BAND\"")]
        public async Task ShouldWriteBandType(BandType bandType, string expectedDescription)
        {
            using (Stream stream = new MemoryStream())
            using (var jsonWriter = new Utf8JsonWriter(stream, default))
            using (var stringReader = new StreamReader(stream))
            {
                bandTypeConverter.Write(jsonWriter, bandType, default);
                await jsonWriter.FlushAsync();

                stream.Seek(0, SeekOrigin.Begin);
                var actualDescription = await stringReader.ReadToEndAsync();

                Assert.AreEqual(expectedDescription, actualDescription, "Band type description should match");
            }
        }
    }
}
