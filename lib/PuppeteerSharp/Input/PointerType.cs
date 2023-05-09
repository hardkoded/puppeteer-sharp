using System.Runtime.Serialization;

namespace PuppeteerSharp.Input
{
    // [JsonConverter(typeof(StringEnumConverter))]
    internal enum PointerType
    {
        /// <summary>
        /// Mouse.
        /// </summary>
        [EnumMember(Value = "mouse")]
        Mouse,

        /// <summary>
        /// Pen.
        /// </summary>
        [EnumMember(Value = "pen")]
        Pen,
    }
}
