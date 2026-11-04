using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Specifies the type of page to create.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<CreatePageType>))]
    public enum CreatePageType
    {
        /// <summary>
        /// Create a new tab.
        /// </summary>
        [EnumMember(Value = "tab")]
        Tab,

        /// <summary>
        /// Create a new window.
        /// </summary>
        [EnumMember(Value = "window")]
        Window,
    }
}
