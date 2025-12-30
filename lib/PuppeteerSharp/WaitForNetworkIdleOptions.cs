namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IPage.WaitForNetworkIdleAsync"/>
    public class WaitForNetworkIdleOptions : WaitForOptions
    {
        /// <summary>
        /// How long to wait for no network requests in milliseconds, defaults to 500 milliseconds.
        /// </summary>
        public int? IdleTime { get; set; }

        /// <summary>
        /// Maximum number of concurrent network connections to allow before considering network idle.
        /// Defaults to 0, meaning network is idle when there are no active connections.
        /// </summary>
        public int Concurrency { get; set; }
    }
}
