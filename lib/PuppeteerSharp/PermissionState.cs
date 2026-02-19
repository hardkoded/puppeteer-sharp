using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Permission state for <see cref="IBrowserContext.SetPermissionAsync"/>.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<PermissionState>))]
    public enum PermissionState
    {
        /// <summary>
        /// The permission is granted.
        /// </summary>
        [EnumMember(Value = "granted")]
        Granted,

        /// <summary>
        /// The permission is denied.
        /// </summary>
        [EnumMember(Value = "denied")]
        Denied,

        /// <summary>
        /// The permission is in the prompt state (default).
        /// </summary>
        [EnumMember(Value = "prompt")]
        Prompt,
    }
}
