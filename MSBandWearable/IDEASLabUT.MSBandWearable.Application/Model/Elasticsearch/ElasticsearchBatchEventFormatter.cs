using static Serilog.Sinks.Http.ByteSize;

using Serilog.Sinks.Http.BatchFormatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDEASLabUT.MSBandWearable.Application.Model.Elasticsearch
{
    public class ElasticsearchBatchEventFormatter : BatchFormatter
    {
        private const char EscapeChar = '\\';

        public ElasticsearchBatchEventFormatter(long? eventBodyLimitBytes = 256 * KB) : base(eventBodyLimitBytes)
        {
        }

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

                IEnumerable<string> logs = logEvent.Split(EscapeChar)
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
