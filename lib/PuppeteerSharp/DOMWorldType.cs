using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter<DOMWorldType>))]
    [DefaultEnumValue((int)Other)]
    internal enum DOMWorldType
    {
        /// <summary>
        /// Other type.
        /// </summary>
        Other,

        /// <summary>
        /// Isolated type.
        /// </summary>
        [EnumMember(Value = "isolated")]
        Isolated,

        /// <summary>
        /// Default type.
        /// </summary>
        [EnumMember(Value = "default")]
        Default,
    }
}
