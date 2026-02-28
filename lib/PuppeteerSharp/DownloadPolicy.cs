using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Download policy for the browser.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<DownloadPolicy>))]
    public enum DownloadPolicy
    {
        /// <summary>
        /// Deny all download requests.
        /// </summary>
        [EnumMember(Value = "deny")]
        Deny,

        /// <summary>
        /// Allow all download requests.
        /// </summary>
        [EnumMember(Value = "allow")]
        Allow,

        /// <summary>
        /// Allow all download requests and name files according to their download guids.
        /// </summary>
        [EnumMember(Value = "allowAndName")]
        AllowAndName,

        /// <summary>
        /// Use default browser behavior.
        /// </summary>
        [EnumMember(Value = "default")]
        Default,
    }
}
