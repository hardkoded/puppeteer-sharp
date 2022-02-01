using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// WebBrowserExtensions
    /// </summary>
    public static class WebBrowserExtensions
    {
        /// <summary>
        /// Asynchronously creates a new instance of <see cref="DevToolsContext"/>
        /// </summary>
        /// <param name="chromiumWebBrowser">ChromiumWebBrowser/ChromiumHostControl instance</param>
        /// <param name="ignoreHTTPSerrors">ignore HTTPS errors</param>
        /// <returns>A Task</returns>
        public static Task<DevToolsContext> GetDevToolsContextAsync(this IChromiumWebBrowserBase chromiumWebBrowser, bool ignoreHTTPSerrors = false)
        {
            var browserHost = chromiumWebBrowser.GetBrowserHost();

            if(browserHost == null)
            {
                CefSharp.WebBrowserExtensions.ThrowExceptionIfBrowserHostNull(browserHost);
            }

            var connection = Connection.Attach(new CefSharpConnectionTransport(browserHost));

            return DevToolsContext.GetDevToolsContextAsync(connection, ignoreHTTPSerrors: ignoreHTTPSerrors);
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
            var connection = Connection.Attach(new CefSharpConnectionTransport(chromiumWebBrowser.GetBrowserHost()));

            return Page.GetPageAsync(connection, ignoreHTTPSerrors: ignoreHTTPSerrors);
        }
    }
}
