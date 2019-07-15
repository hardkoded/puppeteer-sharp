namespace PuppeteerSharp
{
    /// <summary>
    /// Browser fetcher options used to construct a <see cref="BrowserFetcher"/>
    /// </summary>
    public class BrowserFetcherOptions
    {
        /// <summary>
        /// Platform, defaults to current platform.
        /// </summary>
        public Platform? Platform { get; set; }
        /// <summary>
        /// A path for the downloads folder. Defaults to [root]/.local-chromium, where [root] is where the project binaries are located.
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// A download host to be used. Defaults to https://storage.googleapis.com.
        /// </summary>
        public string Host { get; set; }
    }
}
