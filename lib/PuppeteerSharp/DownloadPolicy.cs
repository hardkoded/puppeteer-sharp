using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Download policy for controlling download behavior.
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
        /// Allow downloads and name files according to their download GUIDs.
        /// </summary>
        [EnumMember(Value = "allowAndName")]
        AllowAndName,

        /// <summary>
        /// Use default behavior if available.
        /// </summary>
        [EnumMember(Value = "default")]
        Default,
    }
}
