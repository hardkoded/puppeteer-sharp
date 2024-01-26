namespace PuppeteerSharp
{
    /// <summary>
    /// Data returned by the drag methods.
    /// </summary>
    public partial class DragData
    {
        /// <summary>
        /// Drag items.
        /// </summary>
        public DragDataItem[] Items { get; set; }

        /// <summary>
        /// Drag operation.
        /// </summary>
        public DragOperation DragOperationsMask { get; set; }
    }
}
