/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Serilog.Events;
using Serilog.Formatting;

using System;
using System.IO;
using System.Linq;

namespace IDEASLabUT.MSBandWearable.Formatter
{
    /// <summary>
    /// A serilog single event formatter
    /// </summary>
    internal class ElasticsearchEventJsonFormatter : ITextFormatter
    {
        private const char StringSplitChar = '\\';

        private readonly Action<string> invalidAction;

        public ElasticsearchEventJsonFormatter(Action<string> invalidAction)
        {
            this.invalidAction = invalidAction;
        }



        /// <summary>
        /// Formats the given single Serilog event into elastisearch bulk api json data format and writes it into
        /// given textwriter.Since, a log event in Serilog represents only one data event with multiple properties
        /// however we have to format this single data event into 2 seperate json string for elasticsearch bulk api.
        /// 
        /// Follow this document for more details about writing log events in Serilog
        /// <see href="https://github.com/serilog/serilog/wiki/Writing-Log-Events"/>'
        /// 
        /// Usecase scenario:
        /// <code> log.Information("{accelerometer}", accelerometerEvent);</code>
        /// This will create a single log event with properties
        /// <code>Key: accelerometer, Value: accelerometerEvent.ToString();</code>
        /// This gets formatted and written into text writer as
        /// <code>
        /// {"index" : {"_index" : "accelerometer"}}\{"accelerometerX" : 1.9}
        /// </code>
        /// where \ is the split character which will be used to format multiple batched log events
        /// In our context `accelerometer` is the index in which we are trying to add `accelerometerEvent` json string
        /// </summary>
        /// <param name="logEvent">A log event which is going to be formatted into elasticsearch bulk api post json body</param>
        /// <param name="output">An output text writer to write the formatted log event</param>
        /// <seealso cref="ElasticsearchBatchEventFormatter"/>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) 
            {
                NotifyInvalidInput($"{nameof(logEvent)} is null");
                return;
            }

            if (output == null)
            {
                NotifyInvalidInput($"{nameof(output)} is null");
                return;
            }

            var properties = logEvent.Properties;
            if (!properties.Any())
            {
                NotifyInvalidInput("Properties is empty");
                return;
            }

            var eventPair = logEvent.Properties.First();

            // Use ScalarValue in order to avoid formatted string represented by Serilog
            if (!(eventPair.Value is ScalarValue scalarValue))
            {
                // Currently we don't support any other value bar Scalar value
                NotifyInvalidInput($"{eventPair.Value.GetType().Name} is not supported value");
                return;
            }

            if (!(scalarValue.Value is string rawJsonEvent))
            {
                // Value in scalar value should be string
                NotifyInvalidInput($"{(scalarValue.Value == null ? "Null" : scalarValue.Value.GetType().Name)} value inside ScalarValue is not supported value");
                return;
            }

            if (string.IsNullOrWhiteSpace(rawJsonEvent))
            {
                NotifyInvalidInput("Empty value inside ScalarValue is not supported value");
                return;
            }

            output.WriteLine(string.Concat(eventPair.Key, StringSplitChar, rawJsonEvent));
        }
        
        private void NotifyInvalidInput(string reason)
        {
            invalidAction?.Invoke(reason);
        }
    }
}
