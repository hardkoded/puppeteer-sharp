namespace PuppeteerSharp
{
    /// <summary>
    /// Options to be used in <see cref="IPage.EmulateNetworkConditionsAsync(NetworkConditions)"/>.
    /// </summary>
    public class NetworkConditions
    {
        /// <summary>
        /// Key to be used with <see cref="Puppeteer.NetworkConditions()"/>.
        /// </summary>
        public const string Slow3G = "Slow 3G";

        /// <summary>
        /// Key to be used with <see cref="Puppeteer.NetworkConditions()"/>.
        /// </summary>
        public const string Fast3G = "Fast 3G";

        /// <summary>
        /// Key to be used with <see cref="Puppeteer.NetworkConditions()"/>.
        /// </summary>
        public const string Slow4G = "Slow 4G";

        /// <summary>
        /// Key to be used with <see cref="Puppeteer.NetworkConditions()"/>.
        /// </summary>
        public const string Fast4G = "Fast 4G";

        /// <summary>
        /// Emulates the offline mode. Shortcut for <see cref="IPage.SetOfflineModeAsync(bool)"/>.
        /// </summary>
        public bool Offline { get; set; }

        /// <summary>
        /// Download speed (bytes/s). If not provided, download throttling is disabled and only the offline mode is emulated.
        /// </summary>
        public double? Download { get; set; }

        /// <summary>
        /// Upload speed (bytes/s). If not provided, upload throttling is disabled and only the offline mode is emulated.
        /// </summary>
        public double? Upload { get; set; }

        /// <summary>
        /// Latency (ms). If not provided, no latency is emulated.
        /// </summary>
        public double? Latency { get; set; }
    }
}
