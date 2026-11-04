namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IBrowser.LaunchPWAAsync(LaunchPWAOptions)"/>.
    /// </summary>
    public class LaunchPWAOptions
    {
        /// <summary>
        /// Gets or sets the id from the web app's manifest file.
        /// </summary>
        public string ManifestId { get; set; }

        /// <summary>
        /// Gets or sets an optional URL within the app's scope to launch. Defaults to the app's start URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the maximum time in milliseconds to wait for the app's page target to appear.
        /// Defaults to 30 seconds. Pass <c>0</c> to disable the timeout.
        /// </summary>
        public int? Timeout { get; set; }
    }
}
