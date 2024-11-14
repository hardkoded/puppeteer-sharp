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
        /// Maximum number concurrent of network connections to be considered inactive.
        /// </summary>
        public int Concurrency { get; set; }
    }
}
