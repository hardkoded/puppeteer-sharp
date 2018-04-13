namespace PuppeteerSharp
{
    /// <summary>
    /// Options for connecting to an existing browser.
    /// </summary>
    public class ConnectOptions : BrowserOptions
    {
        /// <summary>
        /// The endpoint to connect to.
        /// </summary>
        public string BrowserWSEndpoint { get; set; }

        /// <summary>
        /// Slow mo value.
        /// </summary>
        public int SlowMo { get; set; }

        /// <summary>
        /// Keep alive value.
        /// </summary>
        public int KeepAliveInterval { get; set; } = 30;
    }
}
