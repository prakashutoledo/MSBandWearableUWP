using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace IDEASLabUT.MSBandWearable.Core.Json
{
    /// <summary>
    /// A custom json nano seconds <see cref="DateTime"/> converter
    /// </summary>
    internal class ZonedDateTimeOptionalNanoConverter : JsonConverter
    {
        private static readonly string DateTimeFormatter = "yyyy-MM-dd'T'HH:mm:ss.ffffffzzz";

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            if (typeof(DateTime).IsAssignableFrom(objectType))
            {
                return true;
            }

            return typeof(DateTimeOffset).IsAssignableFrom(objectType);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            reader.DateFormatString = DateTimeFormatter;
            return reader.Value;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string dateTimeString = value is DateTime time
                ? time.ToString(DateTimeFormatter)
                : value is DateTimeOffset offset
                    ? offset.DateTime.ToString(DateTimeFormatter)
                    : throw new Exception("Cannot convert to datetime string");

            // this will remove colon (:) character from timezone value to match elasticsearch datetime format and writes the formatted string
            // Actual String   : 2022-04-10T11:23:37.009619-04:00
            // Formated String : 2022-04-10T11:23:37.009619-0400
            writer.WriteValue(dateTimeString.Remove(dateTimeString.Length - 3, 1));
        }
    }
}
