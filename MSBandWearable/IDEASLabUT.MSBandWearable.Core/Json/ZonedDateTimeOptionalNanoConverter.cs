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
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string dateTimeString = value is DateTime time
                ? time.ToString(DateTimeFormatter)
                : value is DateTimeOffset offset
                    ? offset.DateTime.ToString(DateTimeFormatter)
                    : throw new Exception("Cannot convert to datetime string");
            // this will remove hyphen character from timezone value to match elasticsearch datetime format
            writer.WriteValue(dateTimeString.Remove(dateTimeString.Length - 3, 1));
        }
    }
}
