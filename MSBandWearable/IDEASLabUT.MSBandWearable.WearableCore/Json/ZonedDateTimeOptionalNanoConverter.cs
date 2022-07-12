using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using static System.Text.Json.JsonTokenType;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// A custom json nano seconds <see cref="DateTime"/> or <see cref="DateTimeOffset"/> converter
    /// </summary>
    public class ZonedDateTimeOptionalNanoConverter : JsonConverter<DateTime>
    {
        private const string DateTimeFormatter = "yyyy-MM-dd'T'HH:mm:ss.ffffffzzz";

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DateTime).IsAssignableFrom(typeToConvert);
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!CanConvert(typeToConvert))
            {
                throw new ArgumentException($"{typeToConvert.Name} is not convertible to {typeof(DateTime).Name}");
            }

            if (reader.TokenType == None)
            {
                throw new InvalidOperationException("No value has been read by reader");
            }
            return reader.TokenType == Null ? default : DateTime.ParseExact(reader.GetString(), DateTimeFormatter, null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateTimeFormatter));
        }
    }
}
