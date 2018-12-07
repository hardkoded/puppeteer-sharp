using Newtonsoft.Json;

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
        [JsonProperty]
        public TargetType Type { get; internal set; }
        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [JsonProperty]
        public string Url { get; internal set; }
        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        [JsonProperty]
        public string TargetId { get; internal set; }
        /// <summary>
        /// Gets or sets the target browser contextId
        /// </summary>
        [JsonProperty]
        public string BrowserContextId { get; internal set; }
        /// <summary>
        /// Get the target that opened this target
        /// </summary>
        [JsonProperty]
        public string OpenerId { get; internal set; }
    }
}