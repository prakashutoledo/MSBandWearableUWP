using static Serilog.Sinks.Http.ByteSize;

using Serilog.Sinks.Http.BatchFormatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDEASLabUT.MSBandWearable.Core.Model.Elasticsearch
{
    public class ElasticsearchBatchEventFormatter : BatchFormatter
    {
        private const char StringSplitChar = '\\';

        public ElasticsearchBatchEventFormatter(long? eventBodyLimitBytes = 256 * KB) : base(eventBodyLimitBytes)
        {
        }

        /// <summary>
        /// Formats the given enumeration of log events to an Elasticsearch bulk request json request data and gets
        /// written into given output text writer. Each events are in the format of 
        /// <code>
        /// {"index" : {"_index" : "accelerometer"}}\{"accelerometerX" : 1.9}
        /// {"index" : {"_index" : "accelerometer"}}\{"accelerometerX" : 4.9}
        /// {"index" : {"_index" : "accelerometer"}}\{"accelerometerX" : 0.9}
        /// {"index" : {"_index" : "accelerometer"}}\{"accelerometerX" : 2.1}
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
        /// <param name="logEvents"></param>
        /// <param name="output"></param>
        public override void Format(IEnumerable<string> logEvents, TextWriter output)
        {
            if (logEvents == null)
            {
                throw new ArgumentNullException(nameof(logEvents));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!logEvents.Any())
            {
                return;
            }

            foreach (string logEvent in logEvents)
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

                output.WriteLine(logs.First());
                output.WriteLine(logs.Last());
            }
        }
    }
}
