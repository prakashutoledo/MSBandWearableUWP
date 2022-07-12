using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Text.Json;

using static System.Text.Encoding;

namespace IDEASLabUT.MSBandWearable.Json
{
    [TestClass]
    public class ZonedDateTimeOptionalNanoConverterTest : BaseJsonConverterTest<ZonedDateTimeOptionalNanoConverter, DateTime>
    {
        [TestMethod]
        public void ShouldRead()
        {
            VerifyRead("null", default, "Should read default datetime for null json token");
            VerifyRead("");
        }
    }
}
