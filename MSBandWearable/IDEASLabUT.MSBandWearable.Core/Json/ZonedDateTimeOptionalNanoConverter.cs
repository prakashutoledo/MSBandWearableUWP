using static IDEASLabUT.MSBandWearable.Core.Util.MSBandWearableCoreUtil;

using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Core.Json
{
    /// <summary>
    /// A custom json nano seconds <see cref="DateTime"/> or <see cref="DateTimeOffset"/> converter
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
            DateTime writerDateTime;
            if (value is DateTime dateTime)
            {
                writerDateTime = dateTime;
            }
            else if (value is DateTimeOffset dateTimeOffset)
            {
                writerDateTime = dateTimeOffset.DateTime;
            }
            else
            {
                throw new Exception("Cannot convert to datetime string");
            }

            // this will remove colon (:) character from timezone value to match elasticsearch datetime format and writes the formatted string
            // Actual String   : 2022-04-10T11:23:37.009619-04:00
            // Formated String : 2022-04-10T11:23:37.009619-0400
            writer.WriteValue(writerDateTime.ToString(DateTimeFormatter).RemoveNthCharacterFromLast(3));
        }
    }
}
