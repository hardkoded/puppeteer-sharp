using System.Runtime.Serialization;
using System.Text.Json;

namespace PuppeteerSharp.Input
{
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
