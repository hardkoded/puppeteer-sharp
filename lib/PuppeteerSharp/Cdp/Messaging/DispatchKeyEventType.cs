using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter<DispatchKeyEventType>))]
    internal enum DispatchKeyEventType
    {
        /// <summary>
        /// Key down.
        /// </summary>
        [EnumMember(Value = "keyDown")]
        KeyDown,

        /// <summary>
        /// Raw key down.
        /// </summary>
        [EnumMember(Value = "rawKeyDown")]
        RawKeyDown,

        /// <summary>
        /// Key up.
        /// </summary>
        [EnumMember(Value = "keyUp")]
        KeyUp,
    }
}
