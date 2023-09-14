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
    /// <inheritdoc/>
    public class Browser : IBrowser
    {
        /// <summary>
        /// Time in milliseconds for process to exit gracefully.
        /// </summary>
        private const int CloseTimeout = 5000;

        private readonly ConcurrentDictionary<string, BrowserContext> _contexts;
        private readonly ILogger<Browser> _logger;
        private readonly Func<TargetInfo, bool> _targetFilterCallback;
        private readonly BrowserContext _defaultContext;
        private readonly CustomQueriesManager _customQueriesManager = new();
        private Task _closeTask;

        internal Browser(
            SupportedBrowser browser,
            Connection connection,
            string[] contextIds,
            bool ignoreHTTPSErrors,
            ViewPortOptions defaultViewport,
            LauncherBase launcher,
            Func<TargetInfo, bool> targetFilter = null,
            Func<TargetInfo, bool> isPageTargetFunc = null)
        {
            IgnoreHTTPSErrors = ignoreHTTPSErrors;
            DefaultViewport = defaultViewport;
            Launcher = launcher;
            Connection = connection;
            _targetFilterCallback = targetFilter ?? ((TargetInfo _) => true);
            _logger = Connection.LoggerFactory.CreateLogger<Browser>();
            IsPageTargetFunc =
                isPageTargetFunc ??
                new Func<TargetInfo, bool>((TargetInfo target) =>
                {
                    return
                        target.Type == TargetType.Page ||
                        target.Type == TargetType.BackgroundPage ||
                        target.Type == TargetType.Webview;
                });

            _defaultContext = new BrowserContext(Connection, this, null);
            _contexts = new ConcurrentDictionary<string, BrowserContext>(
                contextIds.Select(contextId =>
                    new KeyValuePair<string, BrowserContext>(contextId, new(Connection, this, contextId))));

            if (browser == SupportedBrowser.Firefox)
            {
                TargetManager = new FirefoxTargetManager(
                        connection,
                        CreateTarget,
                        _targetFilterCallback);
            }
            else
            {
                TargetManager = new ChromeTargetManager(
                    connection,
                    CreateTarget,
                    _targetFilterCallback,
                    launcher?.Options?.Timeout ?? Puppeteer.DefaultTimeout);
            }
        }

        /// <inheritdoc/>
        public event EventHandler Closed;

        /// <inheritdoc/>
        public event EventHandler Disconnected;

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetChanged;

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetCreated;

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetDestroyed;

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        /// <inheritdoc/>
        public string WebSocketEndpoint => Connection.Url;

        /// <inheritdoc/>
        public Process Process => Launcher?.Process;

        /// <inheritdoc/>
        public bool IgnoreHTTPSErrors { get; set; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IBrowserContext DefaultContext => _defaultContext;

        /// <inheritdoc/>
        public int DefaultWaitForTimeout { get; set; } = Puppeteer.DefaultTimeout;

        /// <inheritdoc/>
        public bool IsConnected => !Connection.IsClosed;

        /// <inheritdoc/>
        public ITarget Target => Targets().FirstOrDefault(t => t.Type == TargetType.Browser);

        internal TaskQueue ScreenshotTaskQueue { get; } = new();

        internal Connection Connection { get; }

        internal ViewPortOptions DefaultViewport { get; }

        internal LauncherBase Launcher { get; set; }

        internal CustomQueriesManager CustomQueriesManager => _customQueriesManager;

        internal ITargetManager TargetManager { get; }

        internal Func<TargetInfo, bool> IsPageTargetFunc { get; set; }

        /// <inheritdoc/>
        public Task<IPage> NewPageAsync() => _defaultContext.NewPageAsync();

        /// <inheritdoc/>
        public ITarget[] Targets()
            => TargetManager.GetAvailableTargets().Values.ToArray();

        /// <inheritdoc/>
        public async Task<IBrowserContext> CreateIncognitoBrowserContextAsync()
        {
            var response = await Connection.SendAsync<CreateBrowserContextResponse>("Target.createBrowserContext", null).ConfigureAwait(false);
            var context = new BrowserContext(Connection, this, response.BrowserContextId);
            _contexts.TryAdd(response.BrowserContextId, context);
            return context;
        }

        /// <inheritdoc/>
        public IBrowserContext[] BrowserContexts()
        {
            var contexts = _contexts.Values.ToArray<IBrowserContext>();

            var allContexts = new IBrowserContext[contexts.Length + 1];
            allContexts[0] = _defaultContext;
            contexts.CopyTo(allContexts, 1);
            return allContexts;
        }

        /// <inheritdoc/>
        public async Task<IPage[]> PagesAsync()
            => (await Task.WhenAll(
                BrowserContexts().Select(t => t.PagesAsync())).ConfigureAwait(false))
                .SelectMany(p => p).ToArray();

        /// <inheritdoc/>
        public async Task<string> GetVersionAsync()
            => (await Connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false)).Product;

        /// <inheritdoc/>
        public async Task<string> GetUserAgentAsync()
            => (await Connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false)).UserAgent;

        /// <inheritdoc/>
        public void Disconnect()
        {
            Connection.Dispose();
            Detach();
        }

        /// <inheritdoc/>
        public Task CloseAsync() => _closeTask ?? (_closeTask = CloseCoreAsync());

        /// <inheritdoc/>
        public async Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var timeout = options?.Timeout ?? DefaultWaitForTimeout;
            var targetCompletionSource = new TaskCompletionSource<ITarget>(TaskCreationOptions.RunContinuationsAsynchronously);

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

                var existingTarget = Targets().FirstOrDefault(predicate);
                if (existingTarget != null)
                {
                    return existingTarget;
                }

                return await targetCompletionSource.Task.WithTimeout(timeout).ConfigureAwait(false);
            }
            finally
            {
                TargetCreated -= TargetHandler;
                TargetChanged -= TargetHandler;
            }
        }

        /// <inheritdoc/>
        public void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
            => CustomQueriesManager.RegisterCustomQueryHandler(name, queryHandler);

        /// <inheritdoc/>
        public void UnregisterCustomQueryHandler(string name)
            => CustomQueriesManager.UnregisterCustomQueryHandler(name);

        /// <inheritdoc/>
        public void ClearCustomQueryHandlers()
            => CustomQueriesManager.ClearCustomQueryHandlers();

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
        /// <returns>ValueTask.</returns>
        public async ValueTask DisposeAsync()
        {
            await CloseAsync().ConfigureAwait(false);
            await ScreenshotTaskQueue.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        internal static async Task<Browser> CreateAsync(
            SupportedBrowser browserToCreate,
            Connection connection,
            string[] contextIds,
            bool ignoreHTTPSErrors,
            ViewPortOptions defaultViewPort,
            LauncherBase launcher,
            Func<TargetInfo, bool> targetFilter = null,
            Func<TargetInfo, bool> isPageTargetCallback = null,
            Action<IBrowser> initAction = null)
        {
            var browser = new Browser(
                browserToCreate,
                connection,
                contextIds,
                ignoreHTTPSErrors,
                defaultViewPort,
                launcher,
                targetFilter,
                isPageTargetCallback);

            try
            {
                initAction?.Invoke(browser);

                await browser.AttachAsync().ConfigureAwait(false);
                return browser;
            }
            catch
            {
                await browser.DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }

        internal IEnumerable<string> GetCustomQueryHandlerNames()
            => CustomQueriesManager.GetCustomQueryHandlerNames();

        internal async Task<IPage> CreatePageInContextAsync(string contextId)
        {
            var createTargetRequest = new TargetCreateTargetRequest
            {
                Url = "about:blank",
            };

            if (contextId != null)
            {
                createTargetRequest.BrowserContextId = contextId;
            }

            var targetId = (await Connection.SendAsync<TargetCreateTargetResponse>("Target.createTarget", createTargetRequest)
                .ConfigureAwait(false)).TargetId;
            var target = await TargetManager.GetAvailableTargets().GetItemAsync(targetId).ConfigureAwait(false);
            await target.InitializedTask.ConfigureAwait(false);
            return await target.PageAsync().ConfigureAwait(false);
        }

        internal async Task DisposeContextAsync(string contextId)
        {
            await Connection.SendAsync("Target.disposeBrowserContext", new TargetDisposeBrowserContextRequest
            {
                BrowserContextId = contextId,
            }).ConfigureAwait(false);
            _contexts.TryRemove(contextId, out var _);
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

        private void TargetManager_TargetDiscovered(object sender, TargetChangedArgs e)
            => TargetDiscovered?.Invoke(this, e);

        private void OnTargetChanged(object sender, TargetChangedArgs e)
        {
            var args = new TargetChangedArgs { Target = e.Target };
            TargetChanged?.Invoke(this, args);
            e.Target.BrowserContext.OnTargetChanged(this, args);
        }

        private async void OnDetachedFromTargetAsync(object sender, TargetChangedArgs e)
        {
            try
            {
                e.Target.InitializedTaskWrapper.TrySetResult(false);
                e.Target.CloseTaskWrapper.TrySetResult(true);

                if (await e.Target.InitializedTask.ConfigureAwait(false))
                {
                    var args = new TargetChangedArgs { Target = e.Target };
                    TargetDestroyed?.Invoke(this, args);
                    e.Target.BrowserContext.OnTargetDestroyed(this, args);
                }
            }
            catch (Exception ex)
            {
                var message = $"Browser failed to process Connection Close. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                Connection.Close(message);
            }
        }

        private async void OnAttachedToTargetAsync(object sender, TargetChangedArgs e)
        {
            try
            {
                if (await e.Target.InitializedTask.ConfigureAwait(false))
                {
                    var args = new TargetChangedArgs { Target = e.Target };
                    TargetCreated?.Invoke(this, args);
                    e.Target.BrowserContext.OnTargetCreated(this, args);
                }
            }
            catch (Exception ex)
            {
                var message = $"Browser failed to process Target Available. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
            }
        }

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

            Closed?.Invoke(this, new EventArgs());
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

        private Task AttachAsync()
        {
            Connection.Disconnected += Connection_Disconnected;
            TargetManager.TargetAvailable += OnAttachedToTargetAsync;
            TargetManager.TargetGone += OnDetachedFromTargetAsync;
            TargetManager.TargetChanged += OnTargetChanged;
            TargetManager.TargetDiscovered += TargetManager_TargetDiscovered;
            return TargetManager.InitializeAsync();
        }

        private void Detach()
        {
            Connection.Disconnected -= Connection_Disconnected;
            TargetManager.TargetAvailable -= OnAttachedToTargetAsync;
            TargetManager.TargetGone -= OnDetachedFromTargetAsync;
            TargetManager.TargetChanged -= OnTargetChanged;
            TargetManager.TargetDiscovered -= TargetManager_TargetDiscovered;
        }

        private Target CreateTarget(TargetInfo targetInfo, CDPSession session)
        {
            var browserContextId = targetInfo.BrowserContextId;

            if (!(browserContextId != null && _contexts.TryGetValue(browserContextId, out var context)))
            {
                context = _defaultContext;
            }

            Func<bool, Task<CDPSession>> createSession = (bool isAutoAttachEmulated) => Connection.CreateSessionAsync(targetInfo, isAutoAttachEmulated);

            if (IsPageTargetFunc(targetInfo))
            {
                return new PageTarget(
                    targetInfo,
                    session,
                    context,
                    TargetManager,
                    createSession,
                    IgnoreHTTPSErrors,
                    DefaultViewport,
                    ScreenshotTaskQueue);
            }

            if (targetInfo.Type == TargetType.ServiceWorker || targetInfo.Type == TargetType.SharedWorker)
            {
                return new WorkerTarget(
                    targetInfo,
                    session,
                    context,
                    TargetManager,
                    createSession);
            }

            return new OtherTarget(
                targetInfo,
                session,
                context,
                TargetManager,
                createSession);
        }
    }
}
