/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Serilog.Sinks.Http;

using System.Collections.Generic;
using System.IO;

using static Microsoft.VisualStudio.TestTools.UnitTesting.DynamicDataSourceType;

namespace IDEASLabUT.MSBandWearable.Formatter
{
    /// <summary>
    /// Unit test for <see cref="ElasticsearchBatchEventFormatter"/>
    /// </summary>
    [TestClass]
    public class ElasticsearchBatchEventFormatterTest : BaseFormatterTest
    {
        private IBatchFormatter batchFormatter;

        [TestInitialize]
        public void Setup()
        {
            batchFormatter = new ElasticsearchBatchEventFormatter(InvalidAction);
        }


        [DataTestMethod]
        [DynamicData(nameof(InvalidDataSupplier), Method)]
        public void ShouldNotFormatInvalidInputs(IEnumerable<string> logEvents, TextWriter output, string expectedReason, string message)
        {
            batchFormatter.Format(logEvents, output);
            Assert.IsNotNull(actualReason, "Formatter failed reason is set");
            Assert.AreEqual(expectedReason, actualReason, message);
        }

        [DataTestMethod]
        [DynamicData(nameof(ValidDataSupplier), Method)]
        public void ShouldFormatValidInputs(IEnumerable<string> logEvents, string expectedString)
        {
            using (var stringWriter = new StringWriter())
            {
                batchFormatter.Format(logEvents, stringWriter);
                var actualString = stringWriter.ToString();
                Assert.IsNull(actualReason, "No error occurred");
                Assert.AreEqual(expectedString, actualString, "Formatted log events should match");
            }
        }

        /// <summary>
        /// Valid data supplier for formatting list of events
        /// </summary>
        /// <returns>A collection of valid data</returns>
        private static IEnumerable<object[]> ValidDataSupplier()
        {
            return new List<object[]>
            {
                new object[] {
                    new List<string>()
                    {
                        null,
                        "",
                        "  ",
                        "    ",
                        "invalid",
                        "invalid\\invalid\\invalid",
                        "accelerometer \\ {\"accererometerX\":\"1.2\"} ",
                        " gsr\\{\"resistance\":\"0.1\"}   ",
                        "gyroscope\\{\"angularX\":\"2.1\"}",
                        "heartrate\\{\"value\":\"value\"}",
                        "ibi\\{\"bpm\":\"60\"}",
                        "temperature\\{\"temperature\":\"35.2\"}"
                    },
                    "{\"index\":{\"_index\": \"accelerometer\"}}\r\n" +
                    "{\"accererometerX\":\"1.2\"}\r\n" +
                    "{\"index\":{\"_index\": \"gsr\"}}\r\n" +
                    "{\"resistance\":\"0.1\"}\r\n" +
                    "{\"index\":{\"_index\": \"gyroscope\"}}\r\n" +
                    "{\"angularX\":\"2.1\"}\r\n" +
                    "{\"index\":{\"_index\": \"heartrate\"}}\r\n" +
                    "{\"value\":\"value\"}\r\n" +
                    "{\"index\":{\"_index\": \"ibi\"}}\r\n" +
                    "{\"bpm\":\"60\"}\r\n" +
                    "{\"index\":{\"_index\": \"temperature\"}}\r\n" +
                    "{\"temperature\":\"35.2\"}\r\n"
                }
            };
        }

        /// <summary>
        /// Invalid data supplier for formatting list of batch serilog events
        /// </summary>
        /// <returns>A collection of an invalid data</returns>
        private static IEnumerable<object[]> InvalidDataSupplier()
        {
            return new List<object[]>
            {
                new object[] { null, null, "logEvents is null", "Invalid input as logEvents is null" },
                new object[] { new List<string>(), null, "logEvents is empty", "Invalid input as logEvents is empty" },
                new object[] { new List<string>() { "" }, null, "output is null", "Invalid input as output is null" }
            };
        }
    }
}
