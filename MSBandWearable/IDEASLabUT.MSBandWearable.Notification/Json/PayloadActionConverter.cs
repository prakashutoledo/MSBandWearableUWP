using IDEASLabUT.MSBandWearable.Model.Notification;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// A custom json enum converter for <see cref="PayloadAction"/>
    /// </summary>
    internal class PayloadActionConverter : JsonConverter<PayloadAction>
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(PayloadAction).IsAssignableFrom(objectType);
        }

        /// <inheritdoc />
        public override PayloadAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var payloadAction = PayloadActionExtension.FromDescription(reader.GetString());

            if (payloadAction.HasValue)
            {
                return payloadAction.Value;
            }

            throw new ArgumentException("Unable to convert the given string into Payload Type");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, PayloadAction value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetDescription());
        }
    }
}
