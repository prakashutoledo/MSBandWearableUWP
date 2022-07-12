using IDEASLabUT.MSBandWearable.Model;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// Unit test for <see cref="BandTypeConverter"/>
    /// </summary>
    [TestClass]
    public class BandTypeConverterTest : BaseEnumConverterTest<BandTypeConverter, BandType>
    {
        [DataTestMethod]
        [DataRow("\"MSBAND\"", BandType.MSBand, "MSBand type should be read")]
        [DataRow("\"E4BAND\"", BandType.E4Band, "E4Band type should be read")]
        public void ShouldReadBandType(string description, BandType expectedBandType, string message)
        {
            VerifyRead(description, expectedBandType, message);
        }

        [DataTestMethod]
        [DataRow(BandType.MSBand, "\"MSBAND\"", "MSBand type should be written")]
        [DataRow(BandType.E4Band, "\"E4BAND\"", "E4Band type should be written")]
        public async Task ShouldWriteBandType(BandType bandType, string expectedDescription, string message)
        {
            await VerifyWrite(bandType, expectedDescription, message);
        }
    }
}
