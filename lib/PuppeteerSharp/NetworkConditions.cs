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
        /// Download speed (bytes/s), `-1` to disable.
        /// </summary>
        public double Download { get; set; } = -1;

        /// <summary>
        /// Upload speed (bytes/s), `-1` to disable.
        /// </summary>
        public double Upload { get; set; } = -1;

        /// <summary>
        /// Latency (ms), `0` to disable.
        /// </summary>
        public double Latency { get; set; } = 0;
    }
}
