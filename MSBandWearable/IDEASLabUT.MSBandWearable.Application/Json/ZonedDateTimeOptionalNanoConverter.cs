using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Newtonsoft.Json.Utilities;
using System.Diagnostics;

namespace IDEASLabUT.MSBandWearable.Application.Json
{
    public class ZonedDateTimeOptionalNanoConverter : JsonConverter
    {
        private static readonly string DateTimeFormatter = "yyyy-MM-dd'T'HH:mm:ss.ffffffzzz";

        public override bool CanConvert(Type objectType)
        {
            if (typeof(DateTime).IsAssignableFrom(objectType))
            {
                return true;
            }

            if (typeof(DateTimeOffset).IsAssignableFrom(objectType))
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
           

            var dateTime = reader.Value.ToString();
            if (objectType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(dateTime);
            }

            return DateTime.Parse(dateTime);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;
            if (value is DateTime time)
            {
                text = time.ToString(DateTimeFormatter);
            }
            else
            {
                text = value is DateTimeOffset offset
                    ? offset.DateTime.ToString(DateTimeFormatter)
                    : throw new Exception("Cannot convert to datetime string");
            }
            writer.WriteValue(text.Remove(text.Length - 3, 1));
        }
    }
}
