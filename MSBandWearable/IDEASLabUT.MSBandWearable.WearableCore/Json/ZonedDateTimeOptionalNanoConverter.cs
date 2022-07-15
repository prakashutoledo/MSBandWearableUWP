/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
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
        private const string DateTimeFormatter = "yyyy-MM-dd'T'HH:mm:ss.ffffffzzzz";

        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DateTime).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc/>
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

            switch (reader.TokenType)
            {
                case None:
                    throw new InvalidOperationException("No value has been read by reader");

                case Null:
                    throw new NullReferenceException($"Cannot convert `null` to {typeof(DateTime).Name}");
            }

            var dateTimeString = reader.GetString();
            try
            {
                return DateTime.ParseExact(dateTimeString, DateTimeFormatter, null);
            }
            catch(FormatException)
            {
                throw new InvalidOperationException($"`{dateTimeString}` is not a valid `{DateTimeFormatter}` format");
            }
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateTimeFormatter));
        }
    }
}
