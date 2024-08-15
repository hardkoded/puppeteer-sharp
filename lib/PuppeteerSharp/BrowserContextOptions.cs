namespace PuppeteerSharp
{
    /// <summary>
    /// BrowserContext options.
    /// </summary>
    public class BrowserContextOptions
    {
        /// <summary>
        /// Proxy server with optional port to use for all requests.
        /// Username and password can be set in <see cref="IPage.AuthenticateAsync(Credentials)"/>.
        /// </summary>
        public string ProxyServer { get; set; }

        /// <summary>
        /// Bypass the proxy for the given semi-colon-separated list of hosts.
        /// </summary>
        public string[] ProxyBypassList { get; set; }
    }
}
