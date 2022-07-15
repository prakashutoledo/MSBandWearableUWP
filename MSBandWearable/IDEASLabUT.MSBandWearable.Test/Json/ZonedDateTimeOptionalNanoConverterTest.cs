/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Microsoft.VisualStudio.TestTools.UnitTesting.DynamicDataSourceType;
using static System.DateTimeKind;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// Unit test for <see cref="ZonedDateTimeOptionalNanoConverter"/>
    /// </summary>
    [TestClass]
    public class ZonedDateTimeOptionalNanoConverterTest : BaseJsonConverterTest<ZonedDateTimeOptionalNanoConverter, DateTime>
    {
        [DataTestMethod]
        [DataRow("\"2019-01-20T23:21:29.001211-05:0\"", "`2019-01-20T23:21:29.001211-05:0` is not a valid `yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz` format")]
        [DataRow("\"2019-01-20T23:21:29.001211-:00\"", "`2019-01-20T23:21:29.001211-:00` is not a valid `yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz` format")]
        [DataRow("\"2019-01-20T23:21:29.001211-05:\"", "`2019-01-20T23:21:29.001211-05:` is not a valid `yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz` format")]
        [DataRow("\"2019-01-20T23:21:29001211-:00\"", "`2019-01-20T23:21:29001211-:00` is not a valid `yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz` format")]
        [DataRow("\"2019-01-2023:21:29001211-:00\"", "`2019-01-2023:21:29001211-:00` is not a valid `yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz` format")]
        [DataRow("\"2019012023212900121100\"", "`2019012023212900121100` is not a valid `yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz` format")]
        [DataRow("\"2019012023\"", "`2019012023` is not a valid `yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz` format")]
        [DataRow("\"2019-01-20T23:21:29.00121105:0\"", "`2019-01-20T23:21:29.00121105:0` is not a valid `yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz` format")]
        public void ShouldFailedToReadInvalidDateTimeFormat(string invalidDateTimeString, string expectedMessage)
        {
            var exception = Assert.ThrowsException<InvalidOperationException>(InvalidConverterRead(typeof(DateTime), invalidDateTimeString, true));
            Assert.AreEqual(expectedMessage, exception.Message, "Invalid date format");
        }

        [DataTestMethod]
        [DynamicData(nameof(DateTimeSupplier), Method)]
        public void ShouldReadDateTime(string dateTimeString, DateTime expectedDateTime)
        {
            VerifyRead(dateTimeString, expectedDateTime, "Formatted date time should match each other");
        }

        [DataTestMethod]
        [DynamicData(nameof(FormattedDateTimeSupplier), Method)]
        public async Task ShouldWriteDateTime(DateTime inputDateTime, string expectedPrintedDate)
        {
            await VerifyWrite(inputDateTime, expectedPrintedDate, "Should write formatted date time") ;
        }

        private static IEnumerable<object[]> FormattedDateTimeSupplier()
        {
            return new List<object[]>
            {
                new object[]
                {
                    new DateTime(2018, 7, 13, 13, 22, 30, 001, Local).AddMicroseconds(200), "\"2018-07-13T13:22:30.001200-04:00\""
                },
                new object[]
                {
                    new DateTime(2020, 1, 18, 9, 11, 45, 001, Local).AddMicroseconds(297), "\"2020-01-18T09:11:45.001297-05:00\""
                }
            };
        }

        private static IEnumerable<object[]> DateTimeSupplier()
        {
            return new List<object[]>
            {
                new object[]
                {
                    "\"2019-01-20T23:21:29.001211-05:00\"", new DateTime(2019, 1, 20, 23, 21, 29, 1, Local).AddMicroseconds(211),
                },
                new object[]
                {
                    "\"2022-07-13T13:22:30.111823-00:00\"", new DateTime(2022, 7, 13, 9, 22, 30, 111, Local).AddMicroseconds(823),
                }
            };
        }
    }

    /// <summary>
    /// An extension class for <see cref="DateTime"/>
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// An extension to add microseconds value to this date time
        /// </summary>
        /// <param name="dateTime">A datetime value to add microseconds</param>
        /// <param name="microseconds">A microseconds value to add</param>
        /// <returns></returns>
        public static DateTime AddMicroseconds(this DateTime dateTime, int microseconds)
        {
            return dateTime.AddTicks(microseconds * 10);
        }
    }
}
