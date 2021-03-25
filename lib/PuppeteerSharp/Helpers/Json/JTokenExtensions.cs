using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace PuppeteerSharp.Helpers.Json
{
    /// <summary>
    /// A set of extension methods for JToken
    /// </summary>
    internal static class JTokenExtensions
    {
        /// <summary>
        /// Creates an instance of the specified .NET type from the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <param name="token">Json token</param>
        /// <param name="camelCase">If set to <c>true</c> the CamelCasePropertyNamesContractResolver will be used.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public static T ToObject<T>(this JToken token, bool camelCase)
        {
            if (camelCase)
            {
                return token.ToObject<T>(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
            }

            return token.ToObject<T>();
        }

        /// <summary>
        /// Creates an instance of the specified .NET type from the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <param name="token">Json token</param>
        /// <param name="jsonSerializerSettings">Serializer settings.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public static T ToObject<T>(this JToken token, JsonSerializerSettings jsonSerializerSettings)
            => token.ToObject<T>(JsonSerializer.Create(jsonSerializerSettings));
    }
}
