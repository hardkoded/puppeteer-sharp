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
        [JsonProperty(Constants.TYPE)]
        public TargetType Type { get; internal set; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [JsonProperty(Constants.URL)]
        public string Url { get; internal set; }

        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        [JsonProperty(Constants.TARGET_ID)]
        public string TargetId { get; internal set; }

        /// <summary>
        /// Gets or sets the target browser contextId
        /// </summary>
        [JsonProperty(Constants.BROWSER_CONTEXT_ID)]        
        public string BrowserContextId { get; internal set; }
        
        /// <summary>
        /// Get the target that opened this target
        /// </summary>
        [JsonProperty(Constants.OPENER_ID)]
        public string OpenerId { get; internal set; }
    }
}