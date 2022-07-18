namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="IDevToolsContext.WaitForNetworkIdleAsync"/>
    public class WaitForNetworkIdleOptions
    {
        /// <summary>
        /// Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="IDevToolsContext.DefaultTimeout"/> property.
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// How long to wait for no network requests in milliseconds, defaults to 500 milliseconds.
        /// </summary>
        public int? IdleTime { get; set; }
    }
}
