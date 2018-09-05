using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Provides methods to interact with a browser in Chromium.
    /// </summary>
    /// <example>
    /// An example of using a <see cref="Browser"/> to create a <see cref="Page"/>:
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
    public class Browser : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Browser"/> class.
        /// </summary>
        /// <param name="connection">The connection</param>
        /// <param name="contextIds">The context ids></param>
        /// <param name="ignoreHTTPSErrors">The option to ignoreHTTPSErrors</param>
        /// <param name="setDefaultViewport">The option to setDefaultViewport</param>
        /// <param name="chromiumProcess">The Chromium process</param>
        public Browser(
            Connection connection,
            string[] contextIds,
            bool ignoreHTTPSErrors,
            bool setDefaultViewport,
            ChromiumProcess chromiumProcess)
        {
            Connection = connection;
            IgnoreHTTPSErrors = ignoreHTTPSErrors;
            SetDefaultViewport = setDefaultViewport;
            TargetsMap = new Dictionary<string, Target>();
            ScreenshotTaskQueue = new TaskQueue();
            _defaultContext = new BrowserContext(this, null);
            _contexts = contextIds.ToDictionary(keySelector: contextId => contextId,
                elementSelector: contextId => new BrowserContext(this, contextId));

            Connection.Closed += (object sender, EventArgs e) => Disconnected?.Invoke(this, new EventArgs());
            Connection.MessageReceived += Connect_MessageReceived;

            _chromiumProcess = chromiumProcess;
            _logger = Connection.LoggerFactory.CreateLogger<Browser>();
        }

        #region Private members

        internal readonly Dictionary<string, Target> TargetsMap;

        private readonly Dictionary<string, BrowserContext> _contexts;
        private readonly ILogger<Browser> _logger;
        private readonly BrowserContext _defaultContext;
        private readonly ChromiumProcess _chromiumProcess;
        private Task _closeTask;
        
        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Raised when puppeteer gets disconnected from the Chromium instance. This might happen because one of the following
        /// - Chromium is closed or crashed
        /// - <see cref="Disconnect"/> method was called
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Raised when the url of a target changes
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetChanged;

        /// <summary>
        /// Raised when a target is created, for example when a new page is opened by <c>window.open</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/open"/> or <see cref="NewPageAsync"/>.
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetCreated;

        /// <summary>
        /// Raised when a target is destroyed, for example when a page is closed
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetDestroyed;

        /// <summary>
        /// Gets the Browser websocket url
        /// </summary>
        /// <remarks>
        /// Browser websocket endpoint which can be used as an argument to <see cref="Puppeteer.ConnectAsync(ConnectOptions, ILoggerFactory)"/>.
        /// The format is <c>ws://${host}:${port}/devtools/browser/[id]</c>
        /// You can find the <c>webSocketDebuggerUrl</c> from <c>http://${host}:${port}/json/version</c>.
        /// Learn more about the devtools protocol <see href="https://chromedevtools.github.io/devtools-protocol"/> 
        /// and the browser endpoint <see href="https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target"/>
        /// </remarks>
        public string WebSocketEndpoint => Connection.Url;

        /// <summary>
        /// Gets the spawned browser process. Returns <c>null</c> if the browser instance was created with <see cref="Puppeteer.ConnectAsync(ConnectOptions, ILoggerFactory)"/> method.
        /// </summary>
        public Process Process => _chromiumProcess?.Process;

        /// <summary>
        /// Gets or Sets whether to ignore HTTPS errors during navigation
        /// </summary>
        public bool IgnoreHTTPSErrors { get; set; }

        /// <summary>
        /// Gets a value indicating if the browser is closed
        /// </summary>
        public bool IsClosed => _closeTask != null && _closeTask.IsCompleted && _closeTask.Exception != null;

        internal TaskQueue ScreenshotTaskQueue { get; set; }
        internal Connection Connection { get; }
        internal bool SetDefaultViewport { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new page
        /// </summary>
        /// <returns>Task which resolves to a new <see cref="Page"/> object</returns>
        public Task<Page> NewPageAsync() => _defaultContext.NewPageAsync();

        /// <summary>
        /// Returns An Array of all active targets
        /// </summary>
        /// <returns>An Array of all active targets</returns>
        public Target[] Targets() => TargetsMap.Values.Where(target => target.IsInitialized).ToArray();

        /// <summary>
        /// Creates a new incognito browser context. This won't share cookies/cache with other browser contexts.
        /// </summary>
        /// <returns>Task which resolves to a new <see cref="BrowserContext"/> object</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using(var browser = await Puppeteer.LaunchAsync(new LaunchOptions()))
        /// {
        ///     // Create a new incognito browser context.
        ///     var context = await browser.CreateIncognitoBrowserContextAsync();
        ///     // Create a new page in a pristine context.
        ///     var page = await context.NewPageAsync();
        ///     // Do stuff
        ///     await page.GoToAsync("https://example.com");
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public async Task<BrowserContext> CreateIncognitoBrowserContextAsync()
        {
            var response = await Connection.SendAsync<CreateBrowserContextResponse>("Target.createBrowserContext", new { });
            var context = new BrowserContext(this, response.BrowserContextId);
            _contexts[response.BrowserContextId] = context;
            return context;
        }

        /// <summary>
        /// Returns an array of all open <see cref="BrowserContext"/>. In a newly created browser, this will return a single instance of <see cref="BrowserContext"/>
        /// </summary>
        /// <returns>An array of <see cref="BrowserContext"/> objects</returns>
        public BrowserContext[] BrowserContexts()
        {
            var allContexts = new BrowserContext[_contexts.Count + 1];
            allContexts[0] = _defaultContext;
            _contexts.Values.CopyTo(allContexts, 0);
            return allContexts;
        }

        /// <summary>
        /// Returns a Task which resolves to an array of all open pages.
        /// </summary>
        /// <returns>Task which resolves to an array of all open pages.</returns>
        public async Task<Page[]> PagesAsync()
            => (await Task.WhenAll(Targets().Select(target => target.PageAsync())).ConfigureAwait(false)).Where(x => x != null).ToArray();

        /// <summary>
        /// Gets the browser's version
        /// </summary>
        /// <returns>For headless Chromium, this is similar to <c>HeadlessChrome/61.0.3153.0</c>. For non-headless, this is similar to <c>Chrome/61.0.3153.0</c></returns>
        /// <remarks>
        /// the format of <see cref="GetVersionAsync"/> might change with future releases of Chromium
        /// </remarks>
        public async Task<string> GetVersionAsync()
        {
            dynamic version = await Connection.SendAsync("Browser.getVersion").ConfigureAwait(false);
            return version.product.ToString();
        }

        /// <summary>
        /// Gets the browser's original user agent
        /// </summary>
        /// <returns>Task which resolves to the browser's original user agent</returns>
        /// <remarks>
        /// Pages can override browser user agent with <see cref="Page.SetUserAgentAsync(string)"/>
        /// </remarks>
        public async Task<string> GetUserAgentAsync()
        {
            dynamic version = await Connection.SendAsync("Browser.getVersion").ConfigureAwait(false);
            return version.userAgent.ToString();
        }

        /// <summary>
        /// Disconnects Puppeteer from the browser, but leaves the Chromium process running. After calling <see cref="Disconnect"/>, the browser object is considered disposed and cannot be used anymore
        /// </summary>
        public void Disconnect() => Connection.Dispose();

        /// <summary>
        /// Closes Chromium and all of its pages (if any were opened). The browser object itself is considered disposed and cannot be used anymore
        /// </summary>
        /// <returns>Task</returns>
        public Task CloseAsync() => _closeTask ?? (_closeTask = CloseCoreAsync());

        private async Task CloseCoreAsync()
        {
            try
            {
                try
                {
                    // Initiate graceful browser close operation but don't await it just yet,
                    // because we want to ensure chromium process shutdown first.
                    var browserCloseTask = Connection.SendAsync("Browser.close", null);

                    if (_chromiumProcess != null)
                    {
                        // Notify chromium process that exit is expected, but should be enforced if it
                        // doesn't occur withing the close timeout.
                        var closeTimeout = TimeSpan.FromMilliseconds(5000);
                        await _chromiumProcess.EnsureExitAsync(closeTimeout).ConfigureAwait(false);
                    }

                    // Now we can safely await the browser close operation without risking keeping chromium
                    // process running for indeterminate period.
                    await browserCloseTask.ConfigureAwait(false);
                }
                finally
                {
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                if (_chromiumProcess != null)
                {
                    await _chromiumProcess.KillAsync().ConfigureAwait(false);
                }
            }

            Closed?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Private Methods

        internal void ChangeTarget(Target target)
        {
            var args = new TargetChangedArgs { Target = target };
            TargetChanged?.Invoke(this, args);
            target.BrowserContext.OnTargetChanged(this, args);
        }

        internal async Task<Page> CreatePageInContextAsync(string contextId)
        {
            var args = new Dictionary<string, object> { ["url"] = "about:blank" };
            if (contextId != null)
            {
                args["browserContextId"] = contextId;
            }
            string targetId = (await Connection.SendAsync("Target.createTarget", args)).targetId.ToString();

            var target = TargetsMap[targetId];
            await target.InitializedTask;
            return await target.PageAsync();
        }

        internal async Task DisposeContextAsync(string contextId)
        {
            await Connection.SendAsync("Target.disposeBrowserContext", new { browserContextId = contextId });
            _contexts.Remove(contextId);
        }

        private async void Connect_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Target.targetCreated":
                    await CreateTargetAsync(e.MessageData.ToObject<TargetCreatedResponse>()).ConfigureAwait(false);
                    return;

                case "Target.targetDestroyed":
                    await DestroyTargetAsync(e.MessageData.ToObject<TargetDestroyedResponse>()).ConfigureAwait(false);
                    return;

                case "Target.targetInfoChanged":
                    ChangeTargetInfo(e.MessageData.ToObject<TargetCreatedResponse>());
                    return;
            }
        }

        private void ChangeTargetInfo(TargetCreatedResponse e)
        {
            if (!TargetsMap.ContainsKey(e.TargetInfo.TargetId))
            {
                throw new InvalidTargetException("Target should exists before ChangeTargetInfo");
            }

            var target = TargetsMap[e.TargetInfo.TargetId];
            target.TargetInfoChanged(e.TargetInfo);
        }

        private async Task DestroyTargetAsync(TargetDestroyedResponse e)
        {
            if (!TargetsMap.ContainsKey(e.TargetId))
            {
                throw new InvalidTargetException("Target should exists before DestroyTarget");
            }

            var target = TargetsMap[e.TargetId];
            TargetsMap.Remove(e.TargetId);

            target.CloseTaskWrapper.TrySetResult(true);

            if (await target.InitializedTask.ConfigureAwait(false))
            {
                var args = new TargetChangedArgs { Target = target };
                TargetDestroyed?.Invoke(this, args);
                target.BrowserContext.OnTargetDestroyed(this, args);
            }
        }

        private async Task CreateTargetAsync(TargetCreatedResponse e)
        {
            var targetInfo = e.TargetInfo;
            var browserContextId = targetInfo.BrowserContextId;

            if (!(browserContextId != null && _contexts.TryGetValue(browserContextId, out var context)))
            {
                context = _defaultContext;
            }

            var target = new Target(
                e.TargetInfo,
                info => Connection.CreateSessionAsync(info),
                context);

            if (TargetsMap.ContainsKey(e.TargetInfo.TargetId))
            {
                _logger.LogError("Target should not exist before targetCreated");
            }

            TargetsMap[e.TargetInfo.TargetId] = target;

            if (await target.InitializedTask.ConfigureAwait(false))
            {
                var args = new TargetChangedArgs { Target = target };
                TargetCreated?.Invoke(this, args);
                context.OnTargetCreated(this, args);
            }
        }

        internal static async Task<Browser> CreateAsync(
            Connection connection,
            string[] contextIds,
            bool ignoreHTTPSErrors,
            bool appMode,
            ChromiumProcess chromiumProcess)
        {
            var browser = new Browser(connection, contextIds, ignoreHTTPSErrors, appMode, chromiumProcess);
            await connection.SendAsync("Target.setDiscoverTargets", new
            {
                discover = true
            }).ConfigureAwait(false);

            return browser;
        }
        #endregion

        #region IDisposable

        /// <summary>
        /// Closes <see cref="Connection"/> and any Chromium <see cref="Process"/> that was
        /// created by Puppeteer.
        /// </summary>
        public void Dispose() => _ = CloseAsync();

        #endregion
    }
}