using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Messaging
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum FileChooserAction
    {
        /// <summary>
        /// Accept
        /// </summary>
        [EnumMember(Value = "accept")]
        Accept,

        /// <summary>
        /// Fallback
        /// </summary>
        [EnumMember(Value = "fallback")]
        Fallback,

        /// <summary>
        /// Cancel
        /// </summary>
        [EnumMember(Value = "cancel")]
        Cancel,
    }
}
