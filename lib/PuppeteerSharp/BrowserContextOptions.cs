using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.QueryHandlers;

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
