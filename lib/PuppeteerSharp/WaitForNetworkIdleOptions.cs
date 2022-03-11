namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="Page.WaitForNetworkIdleAsync(WaitForNetworkIdleOptions)"/>
    /// </summary>
    public class WaitForNetworkIdleOptions
    {
        /// <summary>
        /// Idle time to wait.
        /// </summary>
        public int? IdleTime { get; set; }

        /// <summary>
        /// Maximum time to wait for in milliseconds. Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="Page.DefaultTimeout"/> property.
        /// </summary>
        public int? Timeout { get; set; }
    }
}