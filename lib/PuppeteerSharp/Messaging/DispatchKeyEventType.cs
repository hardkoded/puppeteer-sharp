using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Messaging
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum DispatchKeyEventType
    {
        /// <summary>
        /// Key down
        /// </summary>
        [EnumMember(Value = "keyDown")]
        KeyDown,

        /// <summary>
        /// Raw key down
        /// </summary>
        [EnumMember(Value = "rawKeyDown")]
        RawKeyDown,

        /// <summary>
        /// Key up
        /// </summary>
        [EnumMember(Value = "keyUp")]
        KeyUp,
    }
}
