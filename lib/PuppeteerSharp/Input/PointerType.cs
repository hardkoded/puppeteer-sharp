using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Input
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter<PointerType>))]
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
