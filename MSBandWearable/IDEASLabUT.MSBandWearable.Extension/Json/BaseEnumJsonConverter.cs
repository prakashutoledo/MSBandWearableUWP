using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// Base json converter class for all <see cref="Enum"/> class
    /// </summary>
    /// <typeparam name="AnyEnum">A type of Enum</typeparam>
    public class BaseEnumJsonConverter<AnyEnum> : JsonConverter<AnyEnum> where AnyEnum : Enum
    {
        private readonly Func<string, object> enumSupplier;
        private readonly Func<AnyEnum, string> descriptionSupplier;

        /// <summary>
        /// Creates a new instance of <see cref="BaseEnumJsonConverter{AnyEnum}"/>
        /// </summary>
        /// <param name="enumSupplier">An enum supplier from string</param>
        /// <param name="descriptionSupplier">A enum description supplier from enum value</param>
        /// <exception cref="ArgumentNullException">If any of input parameters is null</exception>
        protected BaseEnumJsonConverter(Func<string, object> enumSupplier, Func<AnyEnum, string> descriptionSupplier)
        {
            this.enumSupplier = enumSupplier ?? throw new ArgumentNullException(nameof(enumSupplier));
            this.descriptionSupplier = descriptionSupplier ?? throw new ArgumentNullException(nameof(descriptionSupplier));
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(AnyEnum).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc/>
        public override AnyEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!CanConvert(typeToConvert))
            {
                throw new ArgumentException($"{typeToConvert.Name} is not convertible to {typeof(AnyEnum).Name}");
            }


            if (reader.TokenType == JsonTokenType.None)
            {
                throw new InvalidOperationException("No value has been read by reader");
            }

            var readValue = reader.GetString();
            var anyEnum = enumSupplier.Invoke(readValue);

            if (anyEnum != null)
            {
                return (AnyEnum) anyEnum;
            }

            throw new NullReferenceException($"Cannot convert `{readValue ?? "null"}` to {typeof(AnyEnum).Name}");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, AnyEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(descriptionSupplier.Invoke(value));
        }
    }
}
