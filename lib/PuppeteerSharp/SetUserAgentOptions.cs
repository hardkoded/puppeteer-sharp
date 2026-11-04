namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IPage.SetUserAgentAsync(SetUserAgentOptions)"/>.
    /// </summary>
    public class SetUserAgentOptions
    {
        /// <summary>
        /// Specific user agent to use in this page.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// User agent metadata.
        /// </summary>
        public UserAgentMetadata UserAgentMetadata { get; set; }

        /// <summary>
        /// Platform to set in the user agent override.
        /// </summary>
        public string Platform { get; set; }
    }
}
