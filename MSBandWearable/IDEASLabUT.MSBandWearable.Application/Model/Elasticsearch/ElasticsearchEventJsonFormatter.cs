using System.Linq;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using System.Collections.Generic;

namespace IDEASLabUT.MSBandWearable.Application.Model.Elasticsearch
{
    public class ElasticsearchEventJsonFormatter : ITextFormatter
    {
        private const char DoubleQuoteChar = (char)34;
        private const string StringEscapeChar = "\\";

        public void Format(LogEvent logEvent, TextWriter output)
        {
            KeyValuePair<string, LogEventPropertyValue> eventPair = logEvent.Properties.Select(property => property).First();
            output.Write($"{{\"index\":{{\"_index\": \"{eventPair.Key}\"}}}}");
            output.Write(StringEscapeChar);
            output.WriteLine(eventPair.Value.ToString().Replace(StringEscapeChar, "").TrimStart(DoubleQuoteChar).TrimEnd(DoubleQuoteChar));
        }
    }
}
