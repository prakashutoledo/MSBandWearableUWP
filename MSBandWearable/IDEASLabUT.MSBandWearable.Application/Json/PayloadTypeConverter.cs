using IDEASLabUT.MSBandWearable.Application.Model.Notification;
using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Application.Json
{
    public class PayloadTypeConverter : JsonConverter
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

            string payloadTypeDescription = reader.Value.ToString();

            var payloadType =  PayloadTypeExtension.FromDescription(payloadTypeDescription);

            if (payloadType.HasValue)
            {
                return payloadType.Value;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text = null;
            if (value is PayloadType payloadType)
            {
                text = payloadType.GetDescription();
            }

            writer.WriteValue(text);
        }
    }
}
