namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IPage.RecordAsync(RecordOptions)"/>.
    /// </summary>
    public class RecordOptions
    {
        /// <summary>
        /// File path to save the recording to.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Specifies whether to overwrite the output file if it already exists.
        /// Defaults to <c>true</c>. When <c>false</c>, an existing file causes an <see cref="System.IO.IOException"/>.
        /// </summary>
        public bool? Overwrite { get; set; }

        /// <summary>
        /// Whether to record audio.
        /// Defaults to <c>false</c>.
        /// </summary>
        public bool? Audio { get; set; }

        /// <summary>
        /// Maximum frame width in pixels.
        /// </summary>
        public int? MaxWidth { get; set; }

        /// <summary>
        /// Maximum frame height in pixels.
        /// </summary>
        public int? MaxHeight { get; set; }

        /// <summary>
        /// Maximum frame rate in frames per second.
        /// </summary>
        public int? FrameRate { get; set; }

        /// <summary>
        /// Frame rate in frames per second (alias for <see cref="FrameRate"/>).
        /// </summary>
        public int? Fps { get; set; }
    }
}
