using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target info.
    /// </summary>
    public class TargetInfo
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public TargetType Type { get; set; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets the target browser contextId.
        /// </summary>
        [JsonConverter(typeof(AnyTypeToStringConverter))]
        public string BrowserContextId { get; set; }

        /// <summary>
        /// Get the target that opened this target.
        /// </summary>
        public string OpenerId { get; set; }

        /// <summary>
        /// Gets whether the target is attached.
        /// </summary>
        public bool Attached { get; set; }

        /// <summary>
        /// Provides additional details for specific target types. For example, for
        /// the type of "page", this may be set to "portal" or "prerender".
        /// </summary>
        public string Subtype { get; set; }
    }
}
