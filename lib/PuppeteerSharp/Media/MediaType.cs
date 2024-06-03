using System.Runtime.Serialization;
using System.Text.Json;

namespace PuppeteerSharp.Media
{
    /// <summary>
    /// Media type.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum MediaType
    {
        /// <summary>
        /// Media Print.
        /// </summary>
        Print,

        /// <summary>
        /// Media Screen.
        /// </summary>
        Screen,

        /// <summary>
        /// No media set.
        /// </summary>
        [EnumMember(Value = "")]
        None,
    }
}
