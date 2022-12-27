using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// Tracing options used on <see cref="ITracing.StartAsync(TracingOptions)"/>.
    /// </summary>
    public class TracingOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether Tracing should capture screenshots in the trace.
        /// </summary>
        /// <value>Screenshots option.</value>
        public bool Screenshots { get; set; }

        /// <summary>
        /// A path to write the trace file to.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Specify custom categories to use instead of default.
        /// </summary>
        /// <value>The categories.</value>
        public List<string> Categories { get; set; }
    }
}
