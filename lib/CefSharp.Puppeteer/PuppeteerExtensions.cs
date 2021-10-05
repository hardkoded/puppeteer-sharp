using System.Threading.Tasks;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// WebBrowserExtensions
    /// </summary>
    public static class WebBrowserExtensions
    {
        /// <summary>
        /// Asynchronously creates a new instance of <see cref="Page"/>
        /// </summary>
        /// <param name="chromiumWebBrowser">ChromiumWebBrowser</param>
        /// <param name="ignoreHTTPSerrors">ignore HTTPS errors</param>
        /// <returns>A Task</returns>
        public static Task<Page> GetPuppeteerPageAsync(this IWebBrowser chromiumWebBrowser, bool ignoreHTTPSerrors = false)
        {
            var connection = Connection.Attach(new CefSharpConnectionTransport(chromiumWebBrowser.GetBrowserHost()));

            return Page.GetPageAsync(connection, ignoreHTTPSerrors: ignoreHTTPSerrors);
        }
    }
}
