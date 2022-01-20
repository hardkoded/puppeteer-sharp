using System;
using System.Threading.Tasks;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// Provides methods to interact with a ChromiumWebBrowser instance
    /// </summary>
    [Obsolete("Use DevToolsContext instead")]
    public class Page : DevToolsContext
    {
        private Page(
            Connection client) : base(client)
        {
        }

        /// <summary>
        /// Attach to connection
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="ignoreHTTPSerrors">ignore certificate errors</param>
        /// <returns>Task</returns>
        [Obsolete("Use DevToolsContext.GetDevToolsContextAsync instead")]
        public static async Task<Page> GetPageAsync(Connection connection, bool ignoreHTTPSerrors = false)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var page = new Page(connection);

            await page.InitializeAsync(ignoreHTTPSerrors).ConfigureAwait(false);

            return page;
        }
    }
}
