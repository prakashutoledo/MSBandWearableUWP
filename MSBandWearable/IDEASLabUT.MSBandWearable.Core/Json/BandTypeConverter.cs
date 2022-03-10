using IDEASLabUT.MSBandWearable.Core.Model;
using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Core.Json
{
    class BandTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(BandType).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var payloadType = BandTypeExtension.FromDescription(reader.Value.ToString());
            if (payloadType.HasValue)
            {
                return payloadType.Value;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string description = null;
            if (value is BandType payloadType)
            {
                description = payloadType.GetDescription();
            }
            writer.WriteValue(description);
        }
    }
}
