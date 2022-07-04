using Serilog.Sinks.Http;

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

        /// <summary>
        /// Initializes a new instance of <see cref="ElasticsearchBatchEventFormatter"/>
        /// </summary>
        public ElasticsearchBatchEventFormatter()
        {
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
            if (logEvents == null || output == null || !logEvents.Any())
            {
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

                if (logs == null || !logs.Any() || logs.Count() != 2)
                {
                    continue;
                }

                output.WriteLine(string.Concat(ElasticsearchIndexJsonPrefix, logs.First(), ElasticsearchIndexJsonPostfix));
                output.WriteLine(logs.Last());
            }
        }
    }
}
