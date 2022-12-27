using System.Runtime.Serialization;
using Newtonsoft.Json;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    [JsonConverter(typeof(FlexibleStringEnumConverter), Other)]
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
