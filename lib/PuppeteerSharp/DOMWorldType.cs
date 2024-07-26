using System.Runtime.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    [DefaultEnumValue<DOMWorldType>(Other)]
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
