using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.AspNetFramework
{
    public class AspNetWebSocketTransport : WebSocketTransport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.AspNet.AspNetWebSocketTransport"/> class.
        /// </summary>
        public AspNetWebSocketTransport() : base(false)
        {
        }

        public override async Task InitializeAsync(string url, IConnectionOptions connectionOptions, ILoggerFactory loggerFactory = null)
        {
            await base.InitializeAsync(url, connectionOptions, loggerFactory).ConfigureAwait(false);
            HostingEnvironment.QueueBackgroundWorkItem((cts) => GetResponseAsync());
        }
    }
}
