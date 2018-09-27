using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    /// <summary>
    /// A set of extension methods for JToken
    /// </summary>
    internal static class JTokenExtensions
    {
        /// <summary>
        /// Shortcut for converting the JToken to a string
        /// </summary>
        /// <param name="token">The JToken</param>
        /// <remarks>Returns null if token is null</remarks>
        /// <returns>A string representation of the JToken</returns>
        public static string AsString(this JToken token)
        {
            return token?.Value<string>();
        }
    }
}
