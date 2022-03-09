using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Core.Json
{
    class ZonedDateTimeOptionalNanoConverter : JsonConverter
    {
        private static readonly string DateTimeFormatter = "yyyy-MM-dd'T'HH:mm:ss.ffffffzzz";

        public override bool CanConvert(Type objectType)
        {
            if (typeof(DateTime).IsAssignableFrom(objectType))
            {
                return true;
            }

            return typeof(DateTimeOffset).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
           /*
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var dateTime = reader.Value;
            if (objectType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.Parse(dateTime);
            }

            return DateTime.Parse(dateTime);
            */
            
            throw new NotImplementedException();
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
            // this will remove hyphen character from timezone value to match elasticsearch datetime format
            writer.WriteValue(text.Remove(text.Length - 3, 1));
        }
    }
}
