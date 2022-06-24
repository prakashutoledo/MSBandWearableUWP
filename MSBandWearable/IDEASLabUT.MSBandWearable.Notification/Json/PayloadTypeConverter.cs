﻿using IDEASLabUT.MSBandWearable.Model.Notification;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// A custom json enum converter for <see cref="PayloadType"/>
    /// </summary>
    internal class PayloadTypeConverter : JsonConverter<PayloadType>
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof(PayloadType).IsAssignableFrom(objectType);
        }

        /// <inheritdoc />
        public override PayloadType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var payloadType = PayloadTypeExtension.FromDescription(reader.GetString());
            if (payloadType.HasValue)
            {
                return payloadType.Value;
            }

            throw new ArgumentException("Unable to convert the given string into Payload Type");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, PayloadType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetDescription());
        }
    }
}