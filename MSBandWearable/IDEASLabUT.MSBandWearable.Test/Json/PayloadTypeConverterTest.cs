using IDEASLabUT.MSBandWearable.Model.Notification;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// Unit test for <see cref="PayloadTypeConverter"/>
    /// </summary>
    [TestClass]
    public class PayloadTypeConverterTest : BaseEnumConverterTest<PayloadTypeConverter, PayloadType>
    {
        [DataTestMethod]
        [DataRow("\"MSBand\"", PayloadType.MSBand, "MSBand payload type shall be read")]
        [DataRow("\"E4Band\"", PayloadType.E4Band, "E4Band payload type shall be read")]
        public void ShouldReadPayloadType(string description, PayloadType expectedPayloadType, string message)
        {
            VerifyRead(description, expectedPayloadType, message);
        }

        [DataTestMethod]
        [DataRow(PayloadType.MSBand, "\"MSBand\"", "MSBand payload type should be written")]
        [DataRow(PayloadType.E4Band, "\"E4Band\"", "E4Band payload type should be written")]
        public async Task ShouldWritePayloadType(PayloadType payloadType, string expectedDescription, string message)
        {
            await VerifyWrite(payloadType, expectedDescription, message);
        }
    }
}
