using System.Runtime.Serialization;

namespace PuppeteerSharp.Cdp.Messaging
{
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
