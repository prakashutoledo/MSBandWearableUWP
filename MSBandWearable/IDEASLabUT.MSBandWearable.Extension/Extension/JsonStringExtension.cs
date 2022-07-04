using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using static System.IO.SeekOrigin;
using static System.Text.Json.JsonNamingPolicy;
using static System.Text.Json.Serialization.JsonIgnoreCondition;

namespace IDEASLabUT.MSBandWearable.Extension
{
    /// <summary>
    /// A utility class which sets default options for <see cref="JsonSerializer"/>
    /// and provides an extension method for json seriliazation and deseriliazation.
    /// </summary>
    public static class JsonStringExtension
    {
        private static readonly Lazy<JsonSerializerOptions> JsonSerializerOptions;

        static JsonStringExtension()
        {
            JsonSerializerOptions = new Lazy<JsonSerializerOptions>(() =>
            {
                // Use camel case contract resolver for property
                // Ignore null values which deserializing
                // Definetely not write with indentation, which is the last thing we want here
                return new JsonSerializerOptions
                {
                    PropertyNamingPolicy = CamelCase,
                    DictionaryKeyPolicy = CamelCase,
                    DefaultIgnoreCondition = WhenWritingNull,
                    WriteIndented = false
                };
            });
        }

        private static JsonSerializerOptions DefaultJsonSerializerOptions => JsonSerializerOptions.Value;

        /// <summary>
        /// Json string extension to deserialize back into object of given type <see cref="{T}"/>
        /// </summary>
        /// <typeparam name="T">A type of object to be converted back into</typeparam>
        /// <param name="json">A json string to be deserialized</param>
        /// <returns>A serialized object of type T from given json string</returns>
        public static T FromJson<T>(this string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json, DefaultJsonSerializerOptions);
        }

        /// <summary>
        /// Json string extension to deserialize back into object of given type
        /// </summary>
        /// <param name="json">A json string to be deserialized</param>
        /// <param name="toType">A deserialized object parameter</param>
        /// <returns>A serialized object from given json string</returns>
        public static object FromJson(this string json, in Type toType)
        {
            return JsonSerializer.Deserialize(json, toType, DefaultJsonSerializerOptions);
        }

        /// <summary>
        /// An extension which serializes object to json representation
        /// </summary>
        /// <typeparam name="T">A type of extension parameter</typeparam>
        /// <param name="value">A value of object to be serialized</param>
        /// <returns>A serialized json string representation</returns>
        public static string ToJson<T>(this T value)
        {
            return JsonSerializer.Serialize<object>(value, DefaultJsonSerializerOptions);
        }

        /// <summary>
        /// An extension which serializes object to utf-8 json representation asynchronously
        /// </summary>
        /// <typeparam name="T">A type of extension paramater</typeparam>
        /// <param name="value">A value to be serialized</param>
        /// <returns>A task that can be awaited</returns>
        public static async Task<string> ToJsonAsync<T>(this T value)
        {
            Stream memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync<object>(memoryStream, value, DefaultJsonSerializerOptions).ConfigureAwait(false);
            _ = memoryStream.Seek(0, Begin);
            using (TextReader streamReader = new StreamReader(memoryStream))
            {
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}
