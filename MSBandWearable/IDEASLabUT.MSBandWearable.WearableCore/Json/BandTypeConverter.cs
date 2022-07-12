using IDEASLabUT.MSBandWearable.Model;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// A custom json enum converter for <see cref="BandType"/>
    /// </summary>
    internal class BandTypeConverter : JsonConverter<BandType>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(BandType).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc/>
        public override BandType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!CanConvert(typeToConvert))
            {
                throw new ArgumentException($"{typeToConvert.Name} is not convertible to BandType");
            }

            
            if (reader.TokenType == JsonTokenType.None)
            {
                throw new InvalidOperationException("No value is read by reader");
            }

            var readValue = reader.GetString();
            var bandType = BandTypeExtension.FromDescription(readValue);

            if (bandType.HasValue)
            {
                return bandType.Value;
            }

            throw new NullReferenceException($"Cannot convert `{readValue ?? "null"}` to BandType");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, BandType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetDescription());
        }
    }
}
