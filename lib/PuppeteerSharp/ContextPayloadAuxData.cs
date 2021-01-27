namespace PuppeteerSharp
{
    internal class ContextPayloadAuxData
    {
        /// <summary>
        /// Gets or sets the frame Id.
        /// </summary>
        public string FrameId { get; set; }

        /// <summary>
        /// Gets or sets the is default boolean.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the dom world type.
        /// </summary>
        public DOMWorldType Type { get; set; }
    }
}
