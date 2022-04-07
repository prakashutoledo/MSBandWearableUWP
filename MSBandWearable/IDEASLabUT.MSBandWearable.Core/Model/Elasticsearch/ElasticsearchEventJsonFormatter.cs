using System.Linq;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using System;

namespace IDEASLabUT.MSBandWearable.Core.Model.Elasticsearch
{
    public class ElasticsearchEventJsonFormatter : ITextFormatter
    {
        private const char DoubleQuoteChar = (char)34;
        private const string StringSplitChar = "\\";

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
        /// <exception cref="ArgumentNullException">If logEvent or output is null</exception>
        /// <seealso cref="ElasticsearchBatchEventFormatter"/>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var eventPair = logEvent.Properties.Select(property => property).First();
            output.Write($"{{\"index\":{{\"_index\": \"{eventPair.Key}\"}}}}");
            output.Write(StringSplitChar);

            // Serilog wierdly adds extra double quotes to the string data. So, we are trimming double quote character from start and end
            // Serilog also adds backslash char(\) to every double quote character. We are replacing (\) with an empty character
            // Actual string: "{\"accelerometerX\" : 1.9}"
            // Formatted string: {"accelerometerX" : 1.0}
            output.WriteLine(eventPair.Value.ToString().Replace(StringSplitChar, "").TrimStart(DoubleQuoteChar).TrimEnd(DoubleQuoteChar));
        }
    }
}
