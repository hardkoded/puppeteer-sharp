using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// WebBrowserExtensions
    /// </summary>
    public static class WebBrowserExtensions
    {
        /// <summary>
        /// Asynchronously creates a new instance of <see cref="DevToolsContext"/>. It's reccommended that you only
        /// create a single <see cref="DevToolsContext"/> per ChromiumWebBrowser instance. Store and reuse a single reference.
        /// If you need to create multiple, make sure to Dispose of the previous instance before creating a new instance.
        /// </summary>
        /// <param name="chromiumWebBrowser">ChromiumWebBrowser/ChromiumHostControl instance</param>
        /// <param name="ignoreHTTPSerrors">ignore HTTPS errors</param>
        /// <param name="factory">Logger factory</param>
        /// <returns>A Task</returns>
        public static async Task<DevToolsContext> CreateDevToolsContextAsync(this IChromiumWebBrowserBase chromiumWebBrowser, bool ignoreHTTPSerrors = false, ILoggerFactory factory = null)
        {
            if (chromiumWebBrowser == null)
            {
                throw new ArgumentNullException(nameof(chromiumWebBrowser));
            }

            if (chromiumWebBrowser.IsDisposed)
            {
                throw new ObjectDisposedException(chromiumWebBrowser.GetType().Name);
            }

            DevToolsContext ctx;

            var internalWebBrowser = chromiumWebBrowser as Internals.IWebBrowserInternal;

            if (internalWebBrowser != null)
            {
                ctx = internalWebBrowser.DevToolsContext as DevToolsContext;

                if (ctx != null && !ctx.IsDisposed)
                {
                    // We already have an existing DevToolsContext, reuse it as having
                    // multiple concurrent isn't supported.
                    return ctx;
                }
            }

            var browserHost = chromiumWebBrowser.GetBrowserHost();

            if (browserHost == null)
            {
                CefSharp.WebBrowserExtensions.ThrowExceptionIfBrowserHostNull(browserHost);
            }

            var connection = DevToolsConnection.Attach(new CefSharpConnectionTransport(browserHost), factory);

            ctx = await DevToolsContext.CreateDevToolsContextAsync(connection, ignoreHTTPSerrors: ignoreHTTPSerrors).ConfigureAwait(false);

            if (internalWebBrowser != null)
            {
                internalWebBrowser.DevToolsContext = ctx;
            }

            return ctx;
        }

        /// <summary>
        /// Asynchronously creates a new instance of <see cref="DevToolsContext"/>. It's reccommended that you only
        /// create a single <see cref="DevToolsContext"/> per ChromiumWebBrowser instance. Store and reuse a single reference.
        /// If you need to create multiple, make sure to Dispose of the previous instance before creating a new instance.
        /// </summary>
        /// <param name="chromiumWebBrowser">ChromiumWebBrowser/ChromiumHostControl instance</param>
        /// <param name="ignoreHTTPSerrors">ignore HTTPS errors</param>
        /// <param name="factory">Logger factory</param>
        /// <returns>A Task</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task<DevToolsContext> GetDevToolsContextAsync(this IChromiumWebBrowserBase chromiumWebBrowser, bool ignoreHTTPSerrors = false, ILoggerFactory factory = null)
        {
            return chromiumWebBrowser.CreateDevToolsContextAsync(ignoreHTTPSerrors, factory);
        }

        /// <summary>
        /// Asynchronously creates a new instance of <see cref="DevToolsContext"/>
        /// </summary>
        /// <param name="chromiumWebBrowser">ChromiumWebBrowser</param>
        /// <param name="ignoreHTTPSerrors">ignore HTTPS errors</param>
        /// <returns>A Task</returns>
        [Obsolete("Use GetDevToolsContextAsync instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task<Page> GetPuppeteerPageAsync(this IWebBrowser chromiumWebBrowser, bool ignoreHTTPSerrors = false)
        {
            var connection = DevToolsConnection.Attach(new CefSharpConnectionTransport(chromiumWebBrowser.GetBrowserHost()));

            return Page.GetPageAsync(connection, ignoreHTTPSerrors: ignoreHTTPSerrors);
        }
    }
}
