using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter<DragEventType>))]
    internal enum DragEventType
    {
        /// <summary>
        /// Drag event.
        /// </summary>
        [EnumMember(Value = "dragEnter")]
        DragEnter,

        /// <summary>
        /// Drag over.
        /// </summary>
        [EnumMember(Value = "dragOver")]
        DragOver,

        /// <summary>
        /// Drop.
        /// </summary>
        [EnumMember(Value = "drop")]
        Drop,
    }
}
