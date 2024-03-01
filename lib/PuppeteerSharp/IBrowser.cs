using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Provides methods to interact with a browser.
    /// </summary>
    /// <example>
    /// An example of using a <see cref="IBrowser"/> to create a <see cref="IPage"/>:
    /// <code>
    /// <![CDATA[
    /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
    /// var page = await browser.NewPageAsync();
    /// await page.GoToAsync("https://example.com");
    /// await browser.CloseAsync();
    /// ]]>
    /// </code>
    /// An example of disconnecting from and reconnecting to a <see cref="Browser"/>:
    /// <code>
    /// <![CDATA[
    /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
    /// var browserWSEndpoint = browser.WebSocketEndpoint;
    /// browser.Disconnect();
    /// var browser2 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint });
    /// await browser2.CloseAsync();
    /// ]]>
    /// </code>
    /// </example>
    public interface IBrowser : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Raised when the <see cref="IBrowser"/> gets closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Raised when puppeteer gets disconnected from the Chromium instance. This might happen because one of the following
        /// - Chromium is closed or crashed
        /// - <see cref="Disconnect"/> method was called
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        /// Raised when the url of a target changes
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetChanged;

        /// <summary>
        /// Raised when a target is created, for example when a new page is opened by <c>window.open</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/open"/> or <see cref="NewPageAsync"/>.
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetCreated;

        /// <summary>
        /// Raised when a target is destroyed, for example when a page is closed
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetDestroyed;

        /// <summary>
        /// Raised when a target is discovered
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        /// <summary>
        /// Returns the default browser context. The default browser context can not be closed.
        /// </summary>
        /// <value>The default context.</value>
        IBrowserContext DefaultContext { get; }

        /// <summary>
        /// Returns the browser type. Chrome, Chromium or Firefox.
        /// </summary>
        SupportedBrowser BrowserType { get; }

        /// <summary>
        /// Default wait time in milliseconds. Defaults to 30 seconds.
        /// </summary>
        int DefaultWaitForTimeout { get; set; }

        /// <summary>
        /// Gets or Sets whether to ignore HTTPS errors during navigation.
        /// </summary>
        bool IgnoreHTTPSErrors { get; set; }

        /// <summary>
        /// Gets a value indicating if the browser is closed.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Indicates that the browser is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the spawned browser process. Returns <c>null</c> if the browser instance was created with <see cref="Puppeteer.ConnectAsync(ConnectOptions, ILoggerFactory)"/> method.
        /// </summary>
        Process Process { get; }

        /// <summary>
        /// A target associated with the browser.
        /// </summary>
        ITarget Target { get; }

        /// <summary>
        /// Gets the Browser websocket url.
        /// </summary>
        /// <remarks>
        /// Browser websocket endpoint which can be used as an argument to <see cref="Puppeteer.ConnectAsync(ConnectOptions, ILoggerFactory)"/>.
        /// The format is <c>ws://${host}:${port}/devtools/browser/[id]</c>
        /// You can find the <c>webSocketDebuggerUrl</c> from <c>http://${host}:${port}/json/version</c>.
        /// Learn more about the devtools protocol <see href="https://chromedevtools.github.io/devtools-protocol"/>
        /// and the browser endpoint <see href="https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target"/>.
        /// </remarks>
        string WebSocketEndpoint { get; }

        /// <summary>
        /// Returns an array of all open <see cref="IBrowserContext"/>. In a newly created browser, this will return a single instance of <see cref="IBrowserContext"/>.
        /// </summary>
        /// <returns>An array of <see cref="IBrowserContext"/> objects.</returns>
        IBrowserContext[] BrowserContexts();

        /// <summary>
        /// Closes Chromium and all of its pages (if any were opened). The browser object itself is considered disposed and cannot be used anymore.
        /// </summary>
        /// <returns>Task.</returns>
        Task CloseAsync();

        /// <summary>
        /// Creates a new browser context. This won't share cookies/cache with other browser contexts.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <returns>Task which resolves to a new <see cref="IBrowserContext"/> object.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using(var browser = await Puppeteer.LaunchAsync(new LaunchOptions()))
        /// {
        ///     // Create a new browser context.
        ///     var context = await browser.CreateBrowserContextAsync();
        ///     // Create a new page in a pristine context.
        ///     var page = await context.NewPageAsync();
        ///     // Do stuff
        ///     await page.GoToAsync("https://example.com");
        /// }
        /// ]]>
        /// </code>
        /// </example>
        Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null);

        /// <summary>
        /// Disconnects Puppeteer from the browser, but leaves the process running. After calling <see cref="Disconnect"/>, the browser object is considered disposed and cannot be used anymore.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets the browser's original user agent.
        /// </summary>
        /// <returns>Task which resolves to the browser's original user agent.</returns>
        /// <remarks>
        /// Pages can override browser user agent with <see cref="IPage.SetUserAgentAsync(string, UserAgentMetadata)"/>.
        /// </remarks>
        Task<string> GetUserAgentAsync();

        /// <summary>
        /// Gets the browser's version.
        /// </summary>
        /// <returns>For headless Chromium, this is similar to <c>HeadlessChrome/61.0.3153.0</c>. For non-headless, this is similar to <c>Chrome/61.0.3153.0</c>.</returns>
        /// <remarks>
        /// the format of <see cref="GetVersionAsync"/> might change with future releases of Chromium.
        /// </remarks>
        Task<string> GetVersionAsync();

        /// <summary>
        /// Creates a new page.
        /// </summary>
        /// <returns>Task which resolves to a new <see cref="IPage"/> object.</returns>
        Task<IPage> NewPageAsync();

        /// <summary>
        /// Returns a Task which resolves to an array of all open pages.
        /// Non visible pages, such as <c>"background_page"</c>, will not be listed here. You can find them using <see cref="PuppeteerSharp.Target.PageAsync"/>.
        /// </summary>
        /// <returns>Task which resolves to an array of all open pages inside the Browser.
        /// In case of multiple browser contexts, the method will return an array with all the pages in all browser contexts.
        /// </returns>
        Task<IPage[]> PagesAsync();

        /// <summary>
        /// Returns An Array of all active targets.
        /// </summary>
        /// <returns>An Array of all active targets.</returns>
        ITarget[] Targets();

        /// <summary>
        /// This searches for a target in this specific browser context.
        /// <example>
        /// <code>
        /// <![CDATA[
        /// await page.EvaluateAsync("() => window.open('https://www.example.com/')");
        /// var newWindowTarget = await browserContext.WaitForTargetAsync((target) => target.Url == "https://www.example.com/");
        /// ]]>
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="predicate">A function to be run for every target.</param>
        /// <param name="options">options.</param>
        /// <returns>Resolves to the first target found that matches the predicate function.</returns>
        Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null);

        /// <summary>
        /// Registers a custom query handler.
        /// After registration, the handler can be used everywhere where a selector is
        /// expected by prepending the selection string with `name/`. The name is
        /// only allowed to consist of lower- and upper case latin letters.
        /// </summary>
        /// <example>
        /// Puppeteer.RegisterCustomQueryHandler("text", "{ … }");
        /// var aHandle = await page.QuerySelectorAsync("text/…").
        /// </example>
        /// <param name="name">The name that the custom query handler will be registered under.</param>
        /// <param name="queryHandler">The query handler to register.</param>
        void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler);

        /// <summary>
        /// Unregister a custom query handler.
        /// </summary>
        /// <param name="name">The name of the query handler to unregistered.</param>
        void UnregisterCustomQueryHandler(string name);

        /// <summary>
        /// Clears all registered handlers.
        /// </summary>
        void ClearCustomQueryHandlers();
    }
}
