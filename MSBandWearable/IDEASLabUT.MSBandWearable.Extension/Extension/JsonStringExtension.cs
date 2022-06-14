using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;

namespace IDEASLabUT.MSBandWearable.Extension
{
    /// <summary>
    /// A utility class which sets default settings for <see cref="JsonConvert"/>
    /// and provides an extension method for json seriliazation and deseriliazation.
    /// </summary>
    public static class JsonStringExtension
    {
        static JsonStringExtension()
        {
            // Default json converter settings to ignore null value, unknown properties resolving members in camel case
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        /// <summary>
        /// Json string extension to deserialize back into object of given type <see cref="{T}"/>
        /// </summary>
        /// <typeparam name="T">A type of object to be converted back into</typeparam>
        /// <param name="json">A json string to be deserialized</param>
        /// <returns>A serialized object of type T from given json string</returns>
        public static T FromJson<T>(this string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Json string extension to deserialize back into object of given type
        /// </summary>
        /// <param name="json">A json string to be deserialized</param>
        /// <param name="toType">A deserialized object parameter</param>
        /// <returns>A serialized object from given json string</returns>
        public static object FromJson(this string json, in Type toType)
        {
            return JsonConvert.DeserializeObject(json, toType);
        }

        /// <summary>
        /// An extension which serializes object to json representation
        /// </summary>
        /// <typeparam name="T">A type of extension parameter</typeparam>
        /// <param name="value">A value of object to be serialized</param>
        /// <returns>A serialized json string representation</returns>
        public static string ToJson<T>(this T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
