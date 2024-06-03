using System.Text.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target info.
    /// </summary>
    public class TargetInfo
    {
        internal TargetInfo()
        {
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public TargetType Type { get; internal set; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; internal set; }

        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId { get; internal set; }

        /// <summary>
        /// Gets the target browser contextId.
        /// </summary>
        public string BrowserContextId { get; internal set; }

        /// <summary>
        /// Get the target that opened this target.
        /// </summary>
        public string OpenerId { get; internal set; }

        /// <summary>
        /// Gets whether the target is attached.
        /// </summary>
        public bool Attached { get; internal set; }

        /// <summary>
        /// Provides additional details for specific target types. For example, for
        /// the type of "page", this may be set to "portal" or "prerender".
        /// </summary>
        public string Subtype { get; set; }
    }
}
