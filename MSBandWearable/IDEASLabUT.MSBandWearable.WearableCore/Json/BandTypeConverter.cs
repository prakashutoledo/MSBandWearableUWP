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

        public override BandType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var bandType = BandTypeExtension.FromDescription(reader.GetString());

            if (bandType.HasValue)
            {
                return bandType.Value;
            }

            throw new ArgumentNullException("Cannot convert to band type");
        }

        public override void Write(Utf8JsonWriter writer, BandType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetDescription());
        }
    }
}
