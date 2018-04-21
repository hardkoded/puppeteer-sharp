namespace PuppeteerSharp
{
    /// <summary>
    /// Options for connecting to an existing browser.
    /// </summary>
    public class ConnectOptions : IBrowserOptions
    {
        /// <summary>
        /// Whether to ignore HTTPS errors during navigation. Defaults to false.
        /// </summary>
        public bool IgnoreHTTPSErrors { get; set; }

        /// <summary>
        /// If set to true, sets Headless = false, otherwise, enables automation.
        /// </summary>
        public bool AppMode { get; set; }

        /// <summary>
        /// A browser websocket endpoint to connect to.
        /// </summary>
        public string BrowserWSEndpoint { get; set; }

        /// <summary>
        /// Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.
        /// </summary>
        public int SlowMo { get; set; }

        /// <summary>
        /// Keep alive value.
        /// </summary>
        public int KeepAliveInterval { get; set; } = 30;
    }
}
