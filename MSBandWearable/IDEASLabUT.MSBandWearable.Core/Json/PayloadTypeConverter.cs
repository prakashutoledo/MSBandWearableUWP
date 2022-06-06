using IDEASLabUT.MSBandWearable.Model.Notification;

using Newtonsoft.Json;

using System;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// A custom json enum converter for <see cref="PayloadType"/>
    /// </summary>
    internal class PayloadTypeConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(PayloadType).IsAssignableFrom(objectType);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
