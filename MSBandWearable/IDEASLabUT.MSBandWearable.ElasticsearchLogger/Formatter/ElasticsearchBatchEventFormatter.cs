using Serilog.Sinks.Http;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDEASLabUT.MSBandWearable.Formatter
{
    /// <summary>
    /// A serilog batch event formatter for creating post body for Elasticsearch bulk api a newline delimited JSON (NDJSON)
    /// </summary>
    internal class ElasticsearchBatchEventFormatter : IBatchFormatter
    {
        private const char StringSplitChar = '\\';
        private const string ElasticsearchIndexJsonPrefix = "{\"index\":{\"_index\": \"";
        private const string ElasticsearchIndexJsonPostfix = "\"}}";

        private readonly Action<string> invalidAction;

        /// <summary>
        /// Creates a new instance of <see cref="ElasticsearchBatchEventFormatter"/>
        /// </summary>
        /// <param name="invalidAction">An invalid action to set</param>
        public ElasticsearchBatchEventFormatter(Action<string> invalidAction)
        {
            this.invalidAction = invalidAction;
        }

        /// <summary>
        /// Formats the given enumeration of log events to an Elasticsearch bulk request json request data and gets
        /// written into given output text writer. Each events are in the format of 
        /// <code>
        /// accelerometer\{"accelerometerX" : 1.9}
        /// accelerometer\{"accelerometerX" : 4.9}
        /// accelerometer\{"accelerometerX" : 0.9}
        /// accelerometer\{"accelerometerX" : 2.1}
        /// </code>
        /// This gets formatted by splitting string for character `\` to create two json strings as shown below 
        /// <code>
        /// {"index" : {"_index" : "test"}}
        /// {"accelerometerX" : 1.9}
        /// {"index" : {"_index" : "test"}}
        /// {"accelerometerX" : 4.9}
        /// {"index" : {"_index" : "test"}}
        /// {"accelerometerX" : 0.9}
        /// {"index" : {"_index" : "test"}}
        /// {"accelerometerX" : 2.1}
        /// </code>
        /// <see href="https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-bulk.html"/>
        /// </summary>
        /// <param name="logEvents">A log events to format</param>
        /// <param name="output">A output writer to write formatter log events</param>
        public void Format(IEnumerable<string> logEvents, TextWriter output)
        {
            if (logEvents == null)
            {
                NotifyInvalidInput($"{nameof(logEvents)} is null");
                return;
            }

            if (!logEvents.Any())
            {
                NotifyInvalidInput($"{nameof(logEvents)} is empty");
                return;
            }

            if (output == null)
            {
                NotifyInvalidInput($"{nameof(output)} is null");
                return;
            }

            foreach (var logEvent in logEvents)
            {
                if (string.IsNullOrWhiteSpace(logEvent))
                {
                    continue;
                }
                var logs = logEvent.Split(StringSplitChar)
                    .Where(log => !string.IsNullOrWhiteSpace(log))
                    .Select(log => log.Trim());

                if (!logs.Any() || logs.Count() != 2)
                {
                    continue;
                }

                output.WriteLine(string.Concat(ElasticsearchIndexJsonPrefix, logs.First(), ElasticsearchIndexJsonPostfix));
                output.WriteLine(logs.Last());
            }
        }

        private void NotifyInvalidInput(string reason)
        {
            invalidAction?.Invoke(reason);
        }
    }
}
