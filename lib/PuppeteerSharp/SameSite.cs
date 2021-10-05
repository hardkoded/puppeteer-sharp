using CefSharp.Puppeteer.Helpers.Json;
using Newtonsoft.Json;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// SameSite values in cookies
    /// </summary>
    [JsonConverter(typeof(FlexibleStringEnumConverter), None)]
    public enum SameSite
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Strict
        /// </summary>
        Strict,
        /// <summary>
        /// Lax
        /// </summary>
        Lax,
        /// <summary>
        /// Extended
        /// </summary>
        Extended
    }
}
