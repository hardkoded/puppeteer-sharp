namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IPage.CaptureHeapSnapshotAsync(HeapSnapshotOptions)"/>.
    /// </summary>
    public class HeapSnapshotOptions
    {
        /// <summary>
        /// Gets or sets the file path to save the heap snapshot to.
        /// </summary>
        public string Path { get; set; }
    }
}
