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
        [JsonProperty("type")]
        public TargetType Type { get; internal set; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [JsonProperty("url")]
        public string Url { get; internal set; }

        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        [JsonProperty("targetId")]
        public string TargetId { get; internal set; }

        /// <summary>
        /// Gets or sets the target browser contextId
        /// </summary>
        [JsonProperty("browserContextId")]        
        public string BrowserContextId { get; internal set; }
        
        /// <summary>
        /// Get the target that opened this target
        /// </summary>
        [JsonProperty("openerId")]
        public string OpenerId { get; internal set; }
    }
}