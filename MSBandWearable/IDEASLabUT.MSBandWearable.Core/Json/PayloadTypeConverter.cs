using IDEASLabUT.MSBandWearable.Core.Model.Notification;
using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Core.Json
{
    class PayloadTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PayloadType).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var payloadType =  PayloadTypeExtension.FromDescription(reader.Value.ToString());
            if (payloadType.HasValue)
            {
                return payloadType.Value;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string description = null;
            if (value is PayloadType payloadType)
            {
                description = payloadType.GetDescription();
            }
            writer.WriteValue(description);
        }
    }
}
