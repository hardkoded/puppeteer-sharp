namespace PuppeteerSharp
{
    /// <summary>
    /// Options to be used in <see cref="Page.EmulateNetworkConditionsAsync(NetworkConditions)"/>
    /// </summary>
    public class NetworkConditions
    {
        /// <summary>
        /// Download speed (bytes/s), `-1` to disable
        /// </summary>
        public double Download { get; set; }

        /// <summary>
        /// Upload speed (bytes/s), `-1` to disable
        /// </summary>
        public double Upload { get; set; }

        /// <summary>
        /// Latency (ms), `0` to disable
        /// </summary>
        public double Latency { get; set; }
    }
}
