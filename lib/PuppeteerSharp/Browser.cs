using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public abstract class Browser : IBrowser
    {
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
        public SupportedBrowser BrowserType { get; protected init; }

        /// <inheritdoc/>
        public Process Process => Launcher?.Process;

        /// <inheritdoc/>
        public bool IgnoreHTTPSErrors { get; set; }

        /// <inheritdoc/>
        public abstract bool IsClosed { get; }

        /// <inheritdoc/>
        public IBrowserContext DefaultContext { get; protected set; }

        /// <inheritdoc/>
        public int DefaultWaitForTimeout { get; set; } = Puppeteer.DefaultTimeout;

        /// <inheritdoc/>
        public bool IsConnected => !Connection.IsClosed;

        /// <inheritdoc/>
        public ITarget Target => Targets().FirstOrDefault(t => t.Type == TargetType.Browser);

        internal TaskQueue ScreenshotTaskQueue { get; } = new();

        internal Connection Connection { get; set; }

        internal ViewPortOptions DefaultViewport { get; set; }

        internal LauncherBase Launcher { get; set; }

        internal Func<Target, bool> IsPageTargetFunc { get; set; }

        /// <inheritdoc/>
        public abstract Task<IPage> NewPageAsync();

        /// <inheritdoc/>
        public abstract ITarget[] Targets();

        /// <inheritdoc/>
        public abstract Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null);

        /// <inheritdoc/>
        public abstract IBrowserContext[] BrowserContexts();

        /// <inheritdoc/>
        public async Task<IPage[]> PagesAsync()
            => (await Task.WhenAll(
                BrowserContexts().Select(t => t.PagesAsync())).ConfigureAwait(false))
                .SelectMany(p => p).ToArray();

        /// <inheritdoc/>
        public abstract Task<string> GetVersionAsync();

        /// <inheritdoc/>
        public abstract Task<string> GetUserAgentAsync();

        /// <inheritdoc/>
        public abstract void Disconnect();

        /// <inheritdoc/>
        public abstract Task CloseAsync();

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
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Custom query handler name must not be empty", nameof(name));
            }

            if (queryHandler == null)
            {
                throw new ArgumentNullException(nameof(queryHandler));
            }

            Connection.CustomQuerySelectorRegistry.RegisterCustomQueryHandler(name, queryHandler);
        }

        /// <inheritdoc/>
        public void UnregisterCustomQueryHandler(string name)
            => Connection.CustomQuerySelectorRegistry.UnregisterCustomQueryHandler(name);

        /// <inheritdoc/>
        public void ClearCustomQueryHandlers()
            => Connection.CustomQuerySelectorRegistry.ClearCustomQueryHandlers();

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

        internal IEnumerable<string> GetCustomQueryHandlerNames()
            => Connection.CustomQuerySelectorRegistry.GetCustomQueryHandlerNames();

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
        /// Emits <see cref="Closed"/> event.
        /// </summary>
        protected void OnClosed() => Closed?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Emits <see cref="Disconnected"/> event.
        /// </summary>
        protected void OnDisconnected() => Disconnected?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Emits <see cref="TargetChanged"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected void OnTargetChanged(TargetChangedArgs e) => TargetChanged?.Invoke(this, e);

        /// <summary>
        /// Emits <see cref="TargetCreated"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected void OnTargetCreated(TargetChangedArgs e) => TargetCreated?.Invoke(this, e);

        /// <summary>
        /// Emits <see cref="TargetDestroyed"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected void OnTargetDestroyed(TargetChangedArgs e) => TargetDestroyed?.Invoke(this, e);

        /// <summary>
        /// Emits <see cref="TargetDiscovered"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected void OnTargetDiscovered(TargetChangedArgs e) => TargetDiscovered?.Invoke(this, e);
    }
}
