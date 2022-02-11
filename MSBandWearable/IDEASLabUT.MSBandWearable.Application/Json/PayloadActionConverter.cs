using IDEASLabUT.MSBandWearable.Application.Model.Notification;
using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Application.Json
{
    public class PayloadActionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PayloadAction).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            string payloadActionDescription = reader.Value.ToString();

            var payloadAction = PayloadActionExtension.FromDescription(payloadActionDescription);

            if (payloadAction.HasValue)
            {
                return payloadAction.Value;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text = null;
            if (value is PayloadAction payloadAction)
            {
                text = payloadAction.GetDescription();
            }

            writer.WriteValue(text);
        }
    }
}
