﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using Serilog.Events;
using Serilog.Formatting;
using Serilog.Parsing;

using System;
using System.Collections.Generic;
using System.IO;

using static Serilog.Events.LogEventLevel;
using static Microsoft.VisualStudio.TestTools.UnitTesting.DynamicDataSourceType;

namespace IDEASLabUT.MSBandWearable.Formatter
{
    /// <summary>
    /// Unit test for <see cref="ElasticsearchEventJsonFormatter"/>
    /// </summary>
    [TestClass]
    public class ElasticsearchEventJsonFormatterTest
    {
        private string actualReason;
        private ITextFormatter formatter;

        [TestInitialize]
        public void Setup()
        {
            actualReason = null;
            formatter = new ElasticsearchEventJsonFormatter(InvalidInput);
        }

        [DataTestMethod]
        [DynamicData(nameof(InvalidDataSupplier), Method)]
        public void ShouldNotFormatInvalidLogEvent(LogEvent logEvent, TextWriter textWriter, string expectedReason, string message)
        {
            formatter.Format(logEvent, textWriter);
            Assert.AreEqual(expectedReason, actualReason, message);
        }

        [DataTestMethod]
        [DynamicData(nameof(ValidDataSupplier), Method)]
        public void ShouldFormatLogEvent(string indexName, string rawJsonEvent, string expectedFormattedEvent)
        {
            using(var textWriter = new StringWriter())
            {
                formatter.Format(
                    NewLogEvent(logEvent => logEvent.AddOrUpdateProperty(
                            new LogEventProperty(indexName, new ScalarValue(rawJsonEvent))
                        )
                    ),
                    textWriter
                );
                Assert.IsNull(actualReason, "No error while writing");
                Assert.AreEqual(expectedFormattedEvent, textWriter.ToString(), "Formatted log event");
            }
        }

        /// <summary>
        /// Supplies valid data needed for formatting single serilog log event
        /// </summary>
        /// <returns>A collection valid datas</returns>
        private static IEnumerable<object[]> ValidDataSupplier()
        {
            return new List<object[]>
            {
                new object[] { "accelerometer", "{\"accererometerX\":\"1.2\"}", "accelerometer\\{\"accererometerX\":\"1.2\"}\r\n" },
                new object[] { "gsr", "{\"resistance\":\"0.1\"}", "gsr\\{\"resistance\":\"0.1\"}\r\n" },
                new object[] { "gyroscope", "{\"angularX\":\"2.1\"}", "gyroscope\\{\"angularX\":\"2.1\"}\r\n" },
                new object[] { "heartrate", "{\"value\":\"value\"}", "heartrate\\{\"value\":\"value\"}\r\n" },
                new object[] { "ibi", "{\"bpm\":\"60\"}", "ibi\\{\"bpm\":\"60\"}\r\n" },
            };
        }

        /// <summary>
        /// Supplier for invalid serilog single log event data
        /// </summary>
        /// <returns>A collection of invalid data</returns>
        private static IEnumerable<object[]> InvalidDataSupplier()
        {
            TextWriter stringWriter = new StringWriter();
            return new List<object[]>
            {
                new object[] { null, null, "logEvent is null", "Invalid input as logEvent is null" },
                new object[] { NewLogEvent(null), null, "output is null", "Invalid input as text writer is null" },
                new object[] { NewLogEvent(null), stringWriter, "Properties is empty", "Empty properties in log event" },
                new object[]
                {
                    NewLogEvent(
                        logEvent => logEvent.AddOrUpdateProperty(
                            new LogEventProperty("property", new StructureValue(new List<LogEventProperty>()))
                        )
                    ),
                    stringWriter,
                    "StructureValue is not supported value",
                    "Invalid input as log event property is of type StructureValue which is not supported"
                },
                new object[]
                {
                    NewLogEvent(
                        logEvent => logEvent.AddOrUpdateProperty(
                            new LogEventProperty("property", new ScalarValue(new object()))
                        )
                    ),
                    stringWriter,
                    "Object value inside ScalarValue is not supported value",
                    "Invalid input as log event property is of type ScalarValue of value object is not supported"
                },
                new object[]
                {
                    NewLogEvent(
                        logEvent => logEvent.AddOrUpdateProperty(
                            new LogEventProperty("property", new ScalarValue(null))
                        )
                    ),
                    stringWriter,
                    "Null value inside ScalarValue is not supported value",
                    "Invalid input as log event property is of type ScalarValue of null value is not supported"
                },
                new object[]
                {
                    NewLogEvent(
                        logEvent => logEvent.AddOrUpdateProperty(
                            new LogEventProperty("property", new ScalarValue("  "))
                        )
                    ),
                    stringWriter,
                    "Empty value inside ScalarValue is not supported value",
                    "Invalid input as log event property is of type ScalarValue of empty string is not supported"
                }
            };
        }

        /// <summary>
        /// Creates a new single serilog log event
        /// </summary>
        /// <param name="logEventAction">An action to update newly created log event</param>
        /// <returns>A newly created log event</returns>
        private static LogEvent NewLogEvent(Action<LogEvent> logEventAction)
        {
            var logEvent = new LogEvent(
                default,
                Information,
                null,
                new MessageTemplate(new List<MessageTemplateToken>()),
                new List<LogEventProperty>()
            );
            logEventAction?.Invoke(logEvent);
            return logEvent;
        }

        private void InvalidInput(string reason)
        {
            actualReason = reason;
        }
    }
}
