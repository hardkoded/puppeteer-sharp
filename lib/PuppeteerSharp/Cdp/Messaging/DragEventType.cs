using System.Runtime.Serialization;

namespace PuppeteerSharp.Cdp.Messaging
{
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
