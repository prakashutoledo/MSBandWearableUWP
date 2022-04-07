using IDEASLabUT.MSBandWearable.Core.Model;
using Newtonsoft.Json;
using System;

namespace IDEASLabUT.MSBandWearable.Core.Json
{
    /// <summary>
    /// A custom json enum converter for <see cref="BandType"/>
    /// </summary>
    internal class BandTypeConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(BandType).IsAssignableFrom(objectType);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
