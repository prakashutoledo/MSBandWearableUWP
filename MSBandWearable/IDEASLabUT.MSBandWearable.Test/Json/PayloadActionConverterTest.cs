/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Model.Notification;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// Unit test fro <see cref="PayloadActionConverter"/>
    /// </summary>
    [TestClass]
    public class PayloadActionConverterTest : BaseEnumConverterTest<PayloadActionConverter, PayloadAction>
    {
        [TestMethod]
        public void ShouldReadPayloadAction()
        {
            VerifyRead("\"sendMessage\"", PayloadAction.SendMessage, "SendMessage payload action shall be read");
        }

        [TestMethod]
        public async Task ShouldWritePayloadAction()
        {
            await VerifyWrite(PayloadAction.SendMessage, "\"sendMessage\"", "SendMessage payload action is written");
        }
    }
}
