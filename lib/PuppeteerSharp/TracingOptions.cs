using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// Tracing options used on <see cref="ITracing.StartAsync(TracingOptions)"/>.
    /// </summary>
    public class TracingOptions
    {
        /// <summary>
        /// Gets or sets a path to write the trace file to.
        /// If no path is specified, the trace will not be written to disk, but can
        /// still be retrieved as a string from <see cref="ITracing.StopAsync"/>.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Tracing should capture screenshots in the trace.
        /// </summary>
        /// <value>Screenshots option.</value>
        public bool Screenshots { get; set; }

        /// <summary>
        /// Gets or sets custom categories to use instead of default.
        /// To exclude a category, prefix it with <c>-</c> (e.g., <c>-toplevel</c>).
        /// </summary>
        /// <value>The categories.</value>
        public List<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the size of the trace buffer in kilobytes.
        /// If not specified or zero is passed, the default value of 200 MB
        /// (200,000 KB) is used by Chromium.
        /// </summary>
        /// <value>The buffer size in kilobytes.</value>
        public int? BufferSize { get; set; }
    }
}
