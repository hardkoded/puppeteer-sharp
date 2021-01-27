namespace PuppeteerSharp
{
    internal class ContextPayloadAuxData
    {
        /// <summary>
        /// Gets or sets the frame Id.
        /// </summary>
        public string FrameId { get; set; }

        public bool IsDefault { get; set; }

        public DOMWorldType Type { get; set; }
    }
}
