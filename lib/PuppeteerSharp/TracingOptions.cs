namespace PuppeteerSharp
{
    public class TracingOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether Tracing should captures screenshots in the trace
        /// </summary>
        /// <value>Screenshots option</value>
        public bool Screenshots { get; set; }
        /// <summary>
        /// A path to write the trace file to
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }
        /// <summary>
        /// Specify custom categories to use instead of default.
        /// </summary>
        /// <value>The categories.</value>
        public string[] Categories { get; set; }
    }
}