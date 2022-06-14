using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// Provides methods to interact with a ChromiumWebBrowser instance
    /// </summary>
    [Obsolete("Use DevToolsContext instead")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1724:Type names should not match namespaces", Justification = "Matches Puppeteer naming.")]
    public class Page : DevToolsContext
    {
        private Page(
            DevToolsConnection client) : base(client)
        {
        }

        /// <summary>
        /// Attach to connection
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="ignoreHTTPSerrors">ignore certificate errors</param>
        /// <returns>Task</returns>
        [Obsolete("Use DevToolsContext.GetDevToolsContextAsync instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static async Task<Page> GetPageAsync(DevToolsConnection connection, bool ignoreHTTPSerrors = false)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var page = new Page(connection);

            await page.InitializeAsync().ConfigureAwait(false);

            if (ignoreHTTPSerrors)
            {
                await page.IgnoreCertificateErrorsAsync().ConfigureAwait(false);
            }

            return page;
        }
    }
}
