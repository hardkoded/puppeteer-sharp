using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
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
    public class Browser : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Time in milliseconds for process to exit gracefully.
        /// </summary>
        private const int CloseTimeout = 5000;

        private readonly ConcurrentDictionary<string, BrowserContext> _contexts;
        private readonly ILogger<Browser> _logger;
        private readonly Func<TargetInfo, bool> _targetFilterCallback;
        private readonly CustomQueriesManager _customQueriesManager = new();

        private Task _closeTask;

        internal Browser(
            Connection connection,
            string[] contextIds,
            bool ignoreHTTPSErrors,
            ViewPortOptions defaultViewport,
            LauncherBase launcher,
            Func<TargetInfo, bool> targetFilter)
        {
            Connection = connection;
            IgnoreHTTPSErrors = ignoreHTTPSErrors;
            DefaultViewport = defaultViewport;
            TargetsMap = new ConcurrentDictionary<string, Target>();
            ScreenshotTaskQueue = new TaskQueue();
            DefaultContext = new BrowserContext(Connection, this, null);
            _contexts = new ConcurrentDictionary<string, BrowserContext>(contextIds.ToDictionary(
                contextId => contextId,
                contextId => new BrowserContext(Connection, this, contextId)));
            Connection.Disconnected += Connection_Disconnected;
            Connection.MessageReceived += Connect_MessageReceived;

            Launcher = launcher;
            _logger = Connection.LoggerFactory.CreateLogger<Browser>();
            _targetFilterCallback = targetFilter ?? ((TargetInfo _) => true);
        }

        /// <summary>
        /// Raised when the <see cref="Browser"/> gets closed.
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
        public Process Process => Launcher?.Process;

        /// <summary>
        /// Gets or Sets whether to ignore HTTPS errors during navigation
        /// </summary>
        public bool IgnoreHTTPSErrors { get; set; }

        /// <summary>
        /// Gets a value indicating if the browser is closed
        /// </summary>
        public bool IsClosed
        {
            get
            {
                if (Launcher == null)
                {
                    return Connection.IsClosed;
                }

                return _closeTask != null && _closeTask.IsCompleted;
            }
        }

        /// <summary>
        /// Returns the default browser context. The default browser context can not be closed.
        /// </summary>
        /// <value>The default context.</value>
        public BrowserContext DefaultContext { get; }

        /// <summary>
        /// Dafault wait time in milliseconds. Defaults to 30 seconds.
        /// </summary>
        public int DefaultWaitForTimeout { get; set; } = Puppeteer.DefaultTimeout;
        /// <summary>
        /// Indicates that the browser is connected.
        /// </summary>
        public bool IsConnected => !Connection.IsClosed;

        /// <summary>
        /// A target associated with the browser.
        /// </summary>
        public Target Target => Targets().FirstOrDefault(t => t.Type == TargetType.Browser);

        internal TaskQueue ScreenshotTaskQueue { get; set; }

        internal Connection Connection { get; }

        internal ViewPortOptions DefaultViewport { get; }

        internal LauncherBase Launcher { get; set; }

        internal IDictionary<string, Target> TargetsMap { get; }

        internal CustomQueriesManager CustomQueriesManager => _customQueriesManager;

        /// <summary>
        /// Creates a new page
        /// </summary>
        /// <returns>Task which resolves to a new <see cref="Page"/> object</returns>
        public Task<Page> NewPageAsync() => DefaultContext.NewPageAsync();

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
            var response = await Connection.SendAsync<CreateBrowserContextResponse>("Target.createBrowserContext", null).ConfigureAwait(false);
            var context = new BrowserContext(Connection, this, response.BrowserContextId);
            _contexts.TryAdd(response.BrowserContextId, context);
            return context;
        }

        /// <summary>
        /// Returns an array of all open <see cref="BrowserContext"/>. In a newly created browser, this will return a single instance of <see cref="BrowserContext"/>
        /// </summary>
        /// <returns>An array of <see cref="BrowserContext"/> objects</returns>
        public BrowserContext[] BrowserContexts()
        {
            var allContexts = new BrowserContext[_contexts.Count + 1];
            allContexts[0] = DefaultContext;
            _contexts.Values.CopyTo(allContexts, 1);
            return allContexts;
        }

        /// <summary>
        /// Returns a Task which resolves to an array of all open pages.
        /// Non visible pages, such as <c>"background_page"</c>, will not be listed here. You can find them using <see cref="PuppeteerSharp.Target.PageAsync"/>
        /// </summary>
        /// <returns>Task which resolves to an array of all open pages inside the Browser.
        /// In case of multiple browser contexts, the method will return an array with all the pages in all browser contexts.
        /// </returns>
        public async Task<Page[]> PagesAsync()
            => (await Task.WhenAll(
                BrowserContexts().Select(t => t.PagesAsync())).ConfigureAwait(false))
                .SelectMany(p => p).ToArray();

        /// <summary>
        /// Gets the browser's version
        /// </summary>
        /// <returns>For headless Chromium, this is similar to <c>HeadlessChrome/61.0.3153.0</c>. For non-headless, this is similar to <c>Chrome/61.0.3153.0</c></returns>
        /// <remarks>
        /// the format of <see cref="GetVersionAsync"/> might change with future releases of Chromium
        /// </remarks>
        public async Task<string> GetVersionAsync()
            => (await Connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false)).Product;

        /// <summary>
        /// Gets the browser's original user agent
        /// </summary>
        /// <returns>Task which resolves to the browser's original user agent</returns>
        /// <remarks>
        /// Pages can override browser user agent with <see cref="Page.SetUserAgentAsync(string, UserAgentMetadata)"/>
        /// </remarks>
        public async Task<string> GetUserAgentAsync()
            => (await Connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false)).UserAgent;

        /// <summary>
        /// Disconnects Puppeteer from the browser, but leaves the process running. After calling <see cref="Disconnect"/>, the browser object is considered disposed and cannot be used anymore
        /// </summary>
        public void Disconnect() => Connection.Dispose();

        /// <summary>
        /// Closes Chromium and all of its pages (if any were opened). The browser object itself is considered disposed and cannot be used anymore
        /// </summary>
        /// <returns>Task</returns>
        public Task CloseAsync() => _closeTask ?? (_closeTask = CloseCoreAsync());

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
        /// <param name="predicate">A function to be run for every target</param>
        /// <param name="options">options</param>
        /// <returns>Resolves to the first target found that matches the predicate function.</returns>
        public async Task<Target> WaitForTargetAsync(Func<Target, bool> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultWaitForTimeout;
            var existingTarget = Targets().FirstOrDefault(predicate);
            if (existingTarget != null)
            {
                return existingTarget;
            }

            var targetCompletionSource = new TaskCompletionSource<Target>(TaskCreationOptions.RunContinuationsAsynchronously);

            void TargetHandler(object sender, TargetChangedArgs e)
            {
                if (predicate(e.Target))
                {
                    targetCompletionSource.TrySetResult(e.Target);
                }
            }

            try
            {
                TargetCreated += TargetHandler;
                TargetChanged += TargetHandler;

                return await targetCompletionSource.Task.WithTimeout(timeout).ConfigureAwait(false);
            }
            finally
            {
                TargetCreated -= TargetHandler;
                TargetChanged -= TargetHandler;
            }
        }

        /// <summary>
        /// Registers a custom query handler.
        /// After registration, the handler can be used everywhere where a selector is
        /// expected by prepending the selection string with `name/`. The name is
        /// only allowed to consist of lower- and upper case latin letters.
        /// </summary>
        /// <example>
        /// Puppeteer.RegisterCustomQueryHandler("text", "{ … }");
        /// var aHandle = await page.QuerySelectorAsync("text/…");
        /// </example>
        /// <param name="name">The name that the custom query handler will be registered under.</param>
        /// <param name="queryHandler">The query handler to register</param>
        public void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
            => CustomQueriesManager.RegisterCustomQueryHandler(name, queryHandler);

        /// <summary>
        /// Returns a list with the names of all registered custom query handlers.
        /// </summary>
        /// <returns>The list of query handlers</returns>
        internal IEnumerable<string> GetCustomQueryHandlerNames()
            => CustomQueriesManager.GetCustomQueryHandlerNames();

        /// <summary>
        /// Unregisters a custom query handler
        /// </summary>
        /// <param name="name">The name of the query handler to unregistered.</param>
        internal void UnregisterCustomQueryHandler(string name)
            => CustomQueriesManager.UnregisterCustomQueryHandler(name);

        /// <summary>
        /// Clears all registered handlers.
        /// </summary>
        internal void ClearCustomQueryHandlers()
            => CustomQueriesManager.ClearCustomQueryHandlers();

        private async Task CloseCoreAsync()
        {
            try
            {
                try
                {
                    // Initiate graceful browser close operation but don't await it just yet,
                    // because we want to ensure process shutdown first.
                    var browserCloseTask = Connection.IsClosed
                        ? Task.CompletedTask
                        : Connection.SendAsync("Browser.close", null);

                    if (Launcher != null)
                    {
                        // Notify process that exit is expected, but should be enforced if it
                        // doesn't occur withing the close timeout.
                        var closeTimeout = TimeSpan.FromMilliseconds(CloseTimeout);
                        await Launcher.EnsureExitAsync(closeTimeout).ConfigureAwait(false);
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

                if (Launcher != null)
                {
                    await Launcher.KillAsync().ConfigureAwait(false);
                }
            }

            // Ensure that remaining targets are always marked closed, so that asynchronous page close
            // operations on any associated pages don't get blocked.
            foreach (var target in TargetsMap.Values)
            {
                target.CloseTaskWrapper.TrySetResult(false);
            }

            Closed?.Invoke(this, new EventArgs());
        }

        internal void ChangeTarget(Target target)
        {
            var args = new TargetChangedArgs { Target = target };
            TargetChanged?.Invoke(this, args);
            target.BrowserContext.OnTargetChanged(this, args);
        }

        internal async Task<Page> CreatePageInContextAsync(string contextId)
        {
            var createTargetRequest = new TargetCreateTargetRequest
            {
                Url = "about:blank"
            };

            if (contextId != null)
            {
                createTargetRequest.BrowserContextId = contextId;
            }

            var targetId = (await Connection.SendAsync<TargetCreateTargetResponse>("Target.createTarget", createTargetRequest)
                .ConfigureAwait(false)).TargetId;
            var target = TargetsMap[targetId];
            await target.InitializedTask.ConfigureAwait(false);
            return await target.PageAsync().ConfigureAwait(false);
        }

        internal async Task DisposeContextAsync(string contextId)
        {
            await Connection.SendAsync("Target.disposeBrowserContext", new TargetDisposeBrowserContextRequest
            {
                BrowserContextId = contextId
            }).ConfigureAwait(false);
            _contexts.TryRemove(contextId, out var _);
        }

        private async void Connection_Disconnected(object sender, EventArgs e)
        {
            try
            {
                await CloseAsync().ConfigureAwait(false);
                Disconnected?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                var message = $"Browser failed to process Connection Close. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                Connection.Close(message);
            }
        }

        private async void Connect_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Target.targetCreated":
                        await CreateTargetAsync(e.MessageData.ToObject<TargetCreatedResponse>(true)).ConfigureAwait(false);
                        return;

                    case "Target.targetDestroyed":
                        await DestroyTargetAsync(e.MessageData.ToObject<TargetDestroyedResponse>(true)).ConfigureAwait(false);
                        return;

                    case "Target.targetInfoChanged":
                        ChangeTargetInfo(e.MessageData.ToObject<TargetCreatedResponse>(true));
                        return;
                }
            }
            catch (Exception ex)
            {
                var message = $"Browser failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                Connection.Close(message);
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

            var shouldAttachToTarget = _targetFilterCallback(targetInfo);
            if (!shouldAttachToTarget)
            {
                return;
            }

            if (!(browserContextId != null && _contexts.TryGetValue(browserContextId, out var context)))
            {
                context = DefaultContext;
            }

            var target = new Target(
                e.TargetInfo,
                () => Connection.CreateSessionAsync(targetInfo),
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
            ViewPortOptions defaultViewPort,
            LauncherBase launcher,
            Func<TargetInfo, bool> targetFilter,
            Action<Browser> initAction = null)
        {
            var browser = new Browser(connection, contextIds, ignoreHTTPSErrors, defaultViewPort, launcher, targetFilter);

            initAction?.Invoke(browser);

            await connection.SendAsync("Target.setDiscoverTargets", new TargetSetDiscoverTargetsRequest
            {
                Discover = true
            }).ConfigureAwait(false);

            return browser;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes <see cref="Connection"/> and any Chromium <see cref="Process"/> that was
        /// created by Puppeteer.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing) => _ = CloseAsync()
            .ContinueWith(
                _ => ScreenshotTaskQueue.DisposeAsync(),
                TaskScheduler.Default);

        /// <summary>
        /// Closes <see cref="Connection"/> and any Chromium <see cref="Process"/> that was
        /// created by Puppeteer.
        /// </summary>
        /// <returns>ValueTask</returns>
        public ValueTask DisposeAsync() => new ValueTask(CloseAsync());
    }
}
