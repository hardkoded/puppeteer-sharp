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
            _defaultContext = new BrowserContext(Connection, this, null);
            _contexts = new ConcurrentDictionary<string, BrowserContext>(contextIds.ToDictionary(
                contextId => contextId,
                contextId => new BrowserContext(Connection, this, contextId)));
            Connection.Disconnected += Connection_Disconnected;
            Connection.MessageReceived += Connect_MessageReceived;

            Launcher = launcher;
            _logger = Connection.LoggerFactory.CreateLogger<Browser>();
            _targetFilterCallback = targetFilter ?? ((TargetInfo _) => true);
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

        internal TaskQueue ScreenshotTaskQueue { get; set; }

        internal Connection Connection { get; }

        internal ViewPortOptions DefaultViewport { get; }

        internal LauncherBase Launcher { get; set; }

        internal IDictionary<string, Target> TargetsMap { get; }

        internal CustomQueriesManager CustomQueriesManager => _customQueriesManager;

        /// <inheritdoc/>
        public Task<IPage> NewPageAsync() => _defaultContext.NewPageAsync();

        /// <inheritdoc/>
        public ITarget[] Targets() => TargetsMap.Values.Where(target => target.IsInitialized).ToArray();

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
        public void Disconnect() => Connection.Dispose();

        /// <inheritdoc/>
        public Task CloseAsync() => _closeTask ?? (_closeTask = CloseCoreAsync());

        /// <inheritdoc/>
        public async Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultWaitForTimeout;
            var existingTarget = Targets().FirstOrDefault(predicate);
            if (existingTarget != null)
            {
                return existingTarget;
            }

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
        public ValueTask DisposeAsync() => new ValueTask(CloseAsync());

        internal static async Task<Browser> CreateAsync(
            Connection connection,
            string[] contextIds,
            bool ignoreHTTPSErrors,
            ViewPortOptions defaultViewPort,
            LauncherBase launcher,
            Func<TargetInfo, bool> targetFilter,
            Action<IBrowser> initAction = null)
        {
            var browser = new Browser(connection, contextIds, ignoreHTTPSErrors, defaultViewPort, launcher, targetFilter);

            initAction?.Invoke(browser);

            await connection.SendAsync("Target.setDiscoverTargets", new TargetSetDiscoverTargetsRequest
            {
                Discover = true,
            }).ConfigureAwait(false);

            return browser;
        }

        /// <inheritdoc/>
        internal IEnumerable<string> GetCustomQueryHandlerNames()
            => CustomQueriesManager.GetCustomQueryHandlerNames();

        internal void ChangeTarget(Target target)
        {
            var args = new TargetChangedArgs { Target = target };
            TargetChanged?.Invoke(this, args);
            ((BrowserContext)target.BrowserContext).OnTargetChanged(this, args);
        }

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
            var target = TargetsMap[targetId];
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
                _ => ScreenshotTaskQueue.DisposeAsync(), TaskScheduler.Default);

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
                ((BrowserContext)target.BrowserContext).OnTargetDestroyed(this, args);
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
                context = _defaultContext;
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
    }
}
