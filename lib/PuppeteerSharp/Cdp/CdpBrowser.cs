// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpBrowser : Browser
{
    /// <summary>
    /// Time in milliseconds for process to exit gracefully.
    /// </summary>
    private const int CloseTimeout = 5000;

    private readonly ConcurrentDictionary<string, CdpBrowserContext> _contexts;
    private readonly ILogger<Browser> _logger;
    private Task _closeTask;

    internal CdpBrowser(
        SupportedBrowser browser,
        Connection connection,
        string[] contextIds,
        ViewPortOptions defaultViewport,
        LauncherBase launcher,
        Func<Target, bool> targetFilter = null,
        Func<Target, bool> isPageTargetFunc = null)
    {
        BrowserType = browser;
        DefaultViewport = defaultViewport;
        Launcher = launcher;
        Connection = connection;
        var targetFilterCallback = targetFilter ?? (_ => true);
        _logger = Connection.LoggerFactory.CreateLogger<Browser>();
        IsPageTargetFunc =
            isPageTargetFunc ??
            (target => target.Type is TargetType.Page or TargetType.BackgroundPage or TargetType.Webview);

        DefaultContext = new CdpBrowserContext(Connection, this, null);
        _contexts = new ConcurrentDictionary<string, CdpBrowserContext>(
            contextIds.Select(contextId =>
                new KeyValuePair<string, CdpBrowserContext>(contextId, new(Connection, this, contextId))));

        if (browser == SupportedBrowser.Firefox)
        {
            TargetManager = new FirefoxTargetManager(
                    connection,
                    CreateTarget,
                    targetFilterCallback);
        }
        else
        {
            TargetManager = new ChromeTargetManager(
                connection,
                CreateTarget,
                targetFilterCallback,
                this,
                launcher?.Options?.Timeout ?? Puppeteer.DefaultTimeout);
        }
    }

    /// <inheritdoc />
    public override bool IsClosed
    {
        get
        {
            if (Launcher == null)
            {
                return Connection.IsClosed;
            }

            return _closeTask is { IsCompleted: true };
        }
    }

    internal ITargetManager TargetManager { get; }

    internal override ProtocolType Protocol => ProtocolType.Cdp;

    /// <inheritdoc/>
    public override Task<IPage> NewPageAsync(CreatePageOptions options = null) => DefaultContext.NewPageAsync(options);

    /// <inheritdoc/>
    public override ITarget[] Targets()
        => TargetManager.GetAvailableTargets().Values.ToArray();

    /// <inheritdoc/>
    public override async Task<string> GetVersionAsync()
        => (await Connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false)).Product;

    /// <inheritdoc/>
    public override async Task<string> GetUserAgentAsync()
        => (await Connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false)).UserAgent;

    /// <inheritdoc/>
    public override void Disconnect()
    {
        Connection.Dispose();
        Detach();
    }

    /// <inheritdoc/>
    public override Task CloseAsync() => _closeTask ??= CloseCoreAsync();

    /// <inheritdoc/>
    public override async Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null)
    {
        var response = await Connection.SendAsync<CreateBrowserContextResponse>(
            "Target.createBrowserContext",
            new TargetCreateBrowserContextRequest
            {
                ProxyServer = options?.ProxyServer,
                ProxyBypassList = string.Join(",", options?.ProxyBypassList ?? Array.Empty<string>()),
            }).ConfigureAwait(false);
        var context = new CdpBrowserContext(Connection, this, response.BrowserContextId);
        _contexts.TryAdd(response.BrowserContextId, context);
        return context;
    }

    /// <inheritdoc/>
    public override IBrowserContext[] BrowserContexts() => [DefaultContext, .. _contexts.Values];

    /// <inheritdoc/>
    public override async Task<WindowBounds> GetWindowBoundsAsync(string windowId)
    {
        var response = await Connection.SendAsync<BrowserGetWindowBoundsResponse>(
            "Browser.getWindowBounds",
            new BrowserGetWindowBoundsRequest { WindowId = int.Parse(windowId, System.Globalization.CultureInfo.InvariantCulture) }).ConfigureAwait(false);
        return response.Bounds;
    }

    /// <inheritdoc/>
    public override async Task SetWindowBoundsAsync(string windowId, WindowBounds windowBounds)
    {
        await Connection.SendAsync(
            "Browser.setWindowBounds",
            new BrowserSetWindowBoundsRequest
            {
                WindowId = int.Parse(windowId, System.Globalization.CultureInfo.InvariantCulture),
                Bounds = windowBounds,
            }).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<ScreenInfo[]> ScreensAsync()
    {
        var response = await Connection.SendAsync<EmulationGetScreenInfosResponse>("Emulation.getScreenInfos").ConfigureAwait(false);
        return response.ScreenInfos;
    }

    /// <inheritdoc/>
    public override async Task<ScreenInfo> AddScreenAsync(AddScreenParams @params)
    {
        var response = await Connection.SendAsync<EmulationAddScreenResponse>("Emulation.addScreen", @params).ConfigureAwait(false);
        return response.ScreenInfo;
    }

    /// <inheritdoc/>
    public override async Task RemoveScreenAsync(string screenId)
    {
        await Connection.SendAsync("Emulation.removeScreen", new EmulationRemoveScreenRequest { ScreenId = screenId }).ConfigureAwait(false);
    }

    internal static async Task<CdpBrowser> CreateAsync(
        SupportedBrowser browserToCreate,
        Connection connection,
        string[] contextIds,
        bool acceptInsecureCerts,
        ViewPortOptions defaultViewPort,
        LauncherBase launcher,
        Func<Target, bool> targetFilter = null,
        Func<Target, bool> isPageTargetCallback = null,
        Action<IBrowser> initAction = null)
    {
        var browser = new CdpBrowser(
            browserToCreate,
            connection,
            contextIds,
            defaultViewPort,
            launcher,
            targetFilter,
            isPageTargetCallback);

        try
        {
            initAction?.Invoke(browser);

            if (acceptInsecureCerts)
            {
                await connection.SendAsync("Security.setIgnoreCertificateErrors", new SecuritySetIgnoreCertificateErrorsRequest { Ignore = true })
                    .ConfigureAwait(false);
            }

            await browser.AttachAsync().ConfigureAwait(false);
            return browser;
        }
        catch
        {
            await browser.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    internal async Task<IPage> CreatePageInContextAsync(string contextId, CreatePageOptions options = null)
    {
        var hasTargets = Array.Exists(Targets(), t => t.BrowserContext.Id == contextId);
        var windowBounds = options?.Type == CreatePageType.Window ? options.WindowBounds : null;

        var createTargetRequest = new TargetCreateTargetRequest
        {
            Url = "about:blank",
            Left = windowBounds?.Left,
            Top = windowBounds?.Top,
            Width = windowBounds?.Width,
            Height = windowBounds?.Height,
            WindowState = windowBounds?.WindowState,

            // Works around crbug.com/454825274.
            NewWindow = hasTargets && options?.Type == CreatePageType.Window ? true : null,
            Background = options?.Background,
        };

        if (contextId != null)
        {
            // We don't have this code in upstream.
            // Puppeteer sends a number if the contextId is a number, even if the typing says that it should be a string.
            // It seems that firefox ignores the contextId if it's not a number. Which is what Firefox sent back.
            createTargetRequest.BrowserContextId = int.TryParse(contextId, out var contextIdAsNumber)
                ? contextIdAsNumber
                : contextId;
        }

        var targetId = (await Connection.SendAsync<TargetCreateTargetResponse>("Target.createTarget", createTargetRequest)
            .ConfigureAwait(false)).TargetId;
        var target = await WaitForTargetAsync(t => ((CdpTarget)t).TargetId == targetId).ConfigureAwait(false) as CdpTarget;
        await target!.InitializedTask.ConfigureAwait(false);
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

    private CdpTarget CreateTarget(TargetInfo targetInfo, CDPSession session, CDPSession parentSession)
    {
        var browserContextId = targetInfo.BrowserContextId;

        if (!(browserContextId != null && _contexts.TryGetValue(browserContextId, out var context)))
        {
            context = (CdpBrowserContext)DefaultContext;
        }

        Task<CDPSession> CreateSession(bool isAutoAttachEmulated) => Connection.CreateSessionAsync(targetInfo, isAutoAttachEmulated);

        var otherTarget = new CdpOtherTarget(
            targetInfo,
            session,
            context,
            TargetManager,
            CreateSession,
            ScreenshotTaskQueue);

        if (targetInfo.Url?.StartsWith("devtools://", StringComparison.OrdinalIgnoreCase) == true)
        {
            return new CdpDevToolsTarget(
                targetInfo,
                session,
                context,
                TargetManager,
                CreateSession,
                DefaultViewport,
                ScreenshotTaskQueue);
        }

        if (IsPageTargetFunc(otherTarget))
        {
            return new CdpPageTarget(
                targetInfo,
                session,
                context,
                TargetManager,
                CreateSession,
                DefaultViewport,
                ScreenshotTaskQueue);
        }

        if (targetInfo.Type == TargetType.ServiceWorker || targetInfo.Type == TargetType.SharedWorker)
        {
            return new CdpWorkerTarget(
                targetInfo,
                session,
                context,
                TargetManager,
                CreateSession,
                this.ScreenshotTaskQueue);
        }

        return otherTarget;
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
                    : Connection.SendAsync("Browser.close");

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

        OnClosed();
    }

    private async void Connection_Disconnected(object sender, EventArgs e)
    {
        try
        {
            await CloseAsync().ConfigureAwait(false);
            OnDisconnected();
        }
        catch (Exception ex)
        {
            var message = $"Browser failed to process Connection Close. {ex.Message}. {ex.StackTrace}";
            _logger.LogError(ex, message);
            Connection.Close(message);
        }
    }

    private void TargetManager_TargetDiscovered(object sender, TargetChangedArgs e)
        => OnTargetDiscovered(e);

    private void OnTargetChanged(object sender, TargetChangedArgs e)
    {
        var args = new TargetChangedArgs(e.Target);
        OnTargetChanged(args);
        ((CdpTarget)e.Target).BrowserContext.OnTargetChanged(args);
    }

    private async void OnDetachedFromTargetAsync(object sender, TargetChangedArgs e)
    {
        try
        {
            var target = (CdpTarget)e.Target;
            target.InitializedTaskWrapper.TrySetResult(InitializationStatus.Aborted);
            target.CloseTaskWrapper.TrySetResult(true);

            if ((await target.InitializedTask.ConfigureAwait(false)) == InitializationStatus.Success)
            {
                var args = new TargetChangedArgs(e.Target);
                OnTargetDestroyed(args);
                e.Target.BrowserContext.OnTargetDestroyed(args);
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
            var target = (CdpTarget)e.Target;
            if (await target.InitializedTask.ConfigureAwait(false) == InitializationStatus.Success)
            {
                var args = new TargetChangedArgs(e.Target);
                OnTargetCreated(args);
                ((CdpTarget)e.Target).BrowserContext.OnTargetCreated(args);
            }
        }
        catch (Exception ex)
        {
            var message = $"Browser failed to process Target Available. {ex.Message}. {ex.StackTrace}";
            _logger.LogError(ex, message);
        }
    }
}
