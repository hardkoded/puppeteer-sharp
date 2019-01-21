using System.Runtime.Serialization;

namespace PuppeteerSharp.Messaging
{
    internal enum DispatchKeyEventType
    {
        [EnumMember(Value = "keyDown")]
        KeyDown,
        [EnumMember(Value = "rawKeyDown")]
        RawKeyDown,
        [EnumMember(Value = "keyUp")]
        KeyUp,
    }
}