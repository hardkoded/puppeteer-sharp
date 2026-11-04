using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// If the user prefers opening an installed web app in a standalone window or in a browser tab.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<PWADisplayMode>))]
    public enum PWADisplayMode
    {
        /// <summary>
        /// The app opens in a standalone window.
        /// </summary>
        [EnumMember(Value = "standalone")]
        Standalone,

        /// <summary>
        /// The app opens in a browser tab.
        /// </summary>
        [EnumMember(Value = "browser")]
        Browser,
    }
}
