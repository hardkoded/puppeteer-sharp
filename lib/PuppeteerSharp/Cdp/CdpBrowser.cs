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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpBrowser : Browser
{
    private readonly ConcurrentDictionary<string, CdpBrowserContext> _contexts;
    private readonly ILogger<Browser> _logger;
    private readonly bool _handleDevToolsAsPage;
    private readonly bool _hasNetworkRestrictions;
    private readonly bool _networkEnabled;
    private readonly Dictionary<string, Extension> _extensions = new();
    private readonly bool _issuesEnabled;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "Disposed in Detach().")]
    private readonly DisposableActionsStack _subscriptions = new();
    private Task _closeTask;
    private Task<BrowserGetVersionResponse> _versionTask;

    internal CdpBrowser(
        SupportedBrowser browser,
        Connection connection,
        string[] contextIds,
        ViewPortOptions defaultViewport,
        LauncherBase launcher,
        Func<Task> closeCallback = null,
        Func<Target, bool> targetFilter = null,
        Func<Target, bool> isPageTargetFunc = null,
        bool handleDevToolsAsPage = false,
        bool networkEnabled = true,
        bool issuesEnabled = true,
        string[] blockList = null,
        string[] allowList = null)
    {
        BrowserType = browser;
        DefaultViewport = defaultViewport;
        Launcher = launcher;
        CloseCallback = closeCallback;
        Connection = connection;
        _handleDevToolsAsPage = handleDevToolsAsPage;
        _networkEnabled = networkEnabled;
        _issuesEnabled = issuesEnabled;
        var targetFilterCallback = targetFilter ?? (_ => true);
        _logger = Connection.LoggerFactory.CreateLogger<Browser>();
        IsPageTargetFunc =
            isPageTargetFunc ??
            (target => target.Type is TargetType.Page or TargetType.BackgroundPage or TargetType.Webview);

        DefaultContext = new CdpBrowserContext(Connection, this, null);
        _contexts = new ConcurrentDictionary<string, CdpBrowserContext>(
            contextIds.Select(contextId =>
                new KeyValuePair<string, CdpBrowserContext>(contextId, new(Connection, this, contextId))));

        _hasNetworkRestrictions = (blockList != null && blockList.Length > 0) || (allowList != null && allowList.Length > 0);
        Connection.RejectEmulateNetworkConditionsCalls = _hasNetworkRestrictions;

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
                blockList,
                allowList);
        }
    }

    /// <inheritdoc />
    public override bool IsClosed
    {
        get
        {
            if (CloseCallback == null)
            {
                return Connection.IsClosed;
            }

            return _closeTask is { IsCompleted: true };
        }
    }

    /// <inheritdoc />
    public override DebugInfo DebugInfo => new()
    {
        PendingProtocolErrors = Connection.GetPendingProtocolErrors(),
    };

    internal ITargetManager TargetManager { get; }

    internal bool HandleDevToolsAsPage => _handleDevToolsAsPage;

    internal override ProtocolType Protocol => ProtocolType.Cdp;

    /// <inheritdoc/>
    public override Task<IPage> NewPageAsync(CreatePageOptions options = null) => DefaultContext.NewPageAsync(options);

    /// <inheritdoc/>
    public override ITarget[] Targets()
        => TargetManager.GetAvailableTargets().Values
            .Where(IsTargetExposed)
            .ToArray();

    /// <inheritdoc/>
    public override async Task<string> GetVersionAsync()
        => (await GetVersionResponseAsync().ConfigureAwait(false)).Product;

    /// <inheritdoc/>
    public override async Task<string> GetUserAgentAsync()
        => (await GetVersionResponseAsync().ConfigureAwait(false)).UserAgent;

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

        if (options?.DownloadBehavior != null)
        {
            await context.SetDownloadBehaviorAsync(options.DownloadBehavior).ConfigureAwait(false);
        }

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

    /// <inheritdoc/>
    public override async Task<string> InstallExtensionAsync(string path, ExtensionInstallOptions options = null)
    {
        var response = await Connection.SendAsync<ExtensionsLoadUnpackedResponse>(
            "Extensions.loadUnpacked",
            new ExtensionsLoadUnpackedRequest { Path = path, EnableInIncognito = options?.EnabledInIncognito ?? false }).ConfigureAwait(false);
        _extensions.Remove(response.Id);
        return response.Id;
    }

    /// <inheritdoc/>
    public override async Task UninstallExtensionAsync(string id)
    {
        await Connection.SendAsync("Extensions.uninstall", new ExtensionsUninstallRequest { Id = id }).ConfigureAwait(false);
        _extensions.Remove(id);
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyDictionary<string, Extension>> ExtensionsAsync()
    {
        var response = await Connection.SendAsync<ExtensionsGetExtensionsResponse>("Extensions.getExtensions")
            .ConfigureAwait(false);

        var extensionsMap = new Dictionary<string, Extension>();

        foreach (var info in response.Extensions)
        {
            if (_extensions.TryGetValue(info.Id, out var existing))
            {
                extensionsMap[info.Id] = existing;
            }
            else
            {
                extensionsMap[info.Id] = new CdpExtension(info.Id, info.Version, info.Name, info.Path, info.Enabled, this);
            }
        }

        _extensions.Clear();
        foreach (var kvp in extensionsMap)
        {
            _extensions[kvp.Key] = kvp.Value;
        }

        return extensionsMap;
    }

    /// <inheritdoc/>
    public override async Task<string> InstallPWAAsync(InstallPWAOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (_hasNetworkRestrictions)
        {
            throw new PuppeteerException("PWA APIs are not supported when network restrictions are configured.");
        }

        await Connection.SendAsync(
            "PWA.install",
            new PWAInstallRequest { ManifestId = options.ManifestId, InstallUrlOrBundleUrl = options.InstallUrlOrBundleUrl }).ConfigureAwait(false);

        if (options.DisplayMode.HasValue)
        {
            await Connection.SendAsync(
                "PWA.changeAppUserSettings",
                new PWAChangeAppUserSettingsRequest { ManifestId = options.ManifestId, DisplayMode = options.DisplayMode }).ConfigureAwait(false);
        }

        return options.ManifestId;
    }

    /// <inheritdoc/>
    public override async Task UninstallPWAAsync(UninstallPWAOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (_hasNetworkRestrictions)
        {
            throw new PuppeteerException("PWA APIs are not supported when network restrictions are configured.");
        }

        await Connection.SendAsync("PWA.uninstall", new PWAUninstallRequest { ManifestId = options.ManifestId }).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IPage> LaunchPWAAsync(LaunchPWAOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (_hasNetworkRestrictions)
        {
            throw new PuppeteerException("PWA APIs are not supported when network restrictions are configured.");
        }

        // `PWA.launch` resolves with the id of the launched *tab* target. Tab targets sit above page targets
        // in the target hierarchy and are not exposed through Targets(), so the returned id can't be awaited
        // directly.
        var response = await Connection.SendAsync<PWALaunchResponse>(
            "PWA.launch",
            new PWALaunchRequest { ManifestId = options.ManifestId, Url = options.Url }).ConfigureAwait(false);

        var target = (CdpTarget)await WaitForTargetAsync(
            candidate =>
            {
                var tab = TargetManager.GetAvailableTargets().GetValueOrDefault(response.TargetId);
                if (tab?.Type != TargetType.Tab)
                {
                    return false;
                }

                return TargetManager.GetChildTargets(tab).Contains(candidate);
            },
            new WaitForOptions { Timeout = options.Timeout }).ConfigureAwait(false);

        // The child page target can be discovered (and match the predicate above) before CDP has reported
        // its real navigated URL, which would otherwise be picked up here with page.Url still empty. Wait
        // for the target to be fully initialized (i.e. its URL populated) before creating the Page for it,
        // matching the pattern used by CreateTargetInPageAsync/GetDevToolsTargetPageAsync.
        await target.InitializedTask.ConfigureAwait(false);

        var page = await target.PageAsync().ConfigureAwait(false);
        if (page == null)
        {
            throw new PuppeteerException($"Failed to create a page for the launched PWA (manifestId = {options.ManifestId})");
        }

        // target.InitializedTask only guarantees the Target domain reported a URL. page.Url is populated by
        // a separate, independently-timed pathway: target.PageAsync() above triggers a Page.getFrameTree
        // call whose response can still carry an empty URL for the main frame even after InitializedTask
        // resolved. If that happens, wait for the main frame to report its navigated URL via the
        // FrameNavigated event before handing the page back, so callers never observe page.Url empty.
        if (string.IsNullOrEmpty(page.Url))
        {
            await page.WaitForFrameAsync(
                frame => frame == page.MainFrame && !string.IsNullOrEmpty(frame.Url),
                new WaitForOptions { Timeout = options.Timeout }).ConfigureAwait(false);
        }

        return page;
    }

    /// <inheritdoc/>
    public override async Task<PWAState> GetPWAStateAsync(GetPWAStateOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (_hasNetworkRestrictions)
        {
            throw new PuppeteerException("PWA APIs are not supported when network restrictions are configured.");
        }

        var response = await Connection.SendAsync<PWAGetOsAppStateResponse>(
            "PWA.getOsAppState",
            new PWAGetOsAppStateRequest { ManifestId = options.ManifestId }).ConfigureAwait(false);

        return new PWAState { BadgeCount = response.BadgeCount, FileHandlers = response.FileHandlers };
    }

    internal static async Task<CdpBrowser> CreateAsync(
        SupportedBrowser browserToCreate,
        Connection connection,
        string[] contextIds,
        bool acceptInsecureCerts,
        ViewPortOptions defaultViewPort,
        LauncherBase launcher,
        Func<Task> closeCallback = null,
        Func<Target, bool> targetFilter = null,
        Func<Target, bool> isPageTargetCallback = null,
        Action<IBrowser> initAction = null,
        bool handleDevToolsAsPage = false,
        bool networkEnabled = true,
        bool issuesEnabled = true,
        string[] blockList = null,
        string[] allowList = null)
    {
        if (allowList != null)
        {
            var versionResponse = await connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion").ConfigureAwait(false);
            var match = Regex.Match(versionResponse.Product, @"\d+");
            if (match.Success && int.TryParse(match.Value, out var majorVersion) && majorVersion < 149)
            {
                throw new PuppeteerException("The Allowlist option requires Chrome 149 or greater.");
            }
        }

        var browser = new CdpBrowser(
            browserToCreate,
            connection,
            contextIds,
            defaultViewPort,
            launcher,
            closeCallback,
            targetFilter,
            isPageTargetCallback,
            handleDevToolsAsPage,
            networkEnabled,
            issuesEnabled,
            blockList,
            allowList);

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

    internal override bool IsNetworkEnabled() => _networkEnabled;

    internal override bool IsIssuesEnabled() => _issuesEnabled;

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

    internal async Task<IPage> CreateDevToolsPageAsync(string pageTargetId)
    {
        var openDevToolsResponse = await Connection.SendAsync<TargetCreateTargetResponse>(
            "Target.openDevTools",
            new TargetActivateTargetRequest { TargetId = pageTargetId }).ConfigureAwait(false);

        return await GetDevToolsTargetPageAsync(openDevToolsResponse.TargetId).ConfigureAwait(false);
    }

    internal async Task<IPage> GetDevToolsTargetPageAsync(string devtoolsTargetId)
    {
        var target = await WaitForTargetAsync(
            t => ((CdpTarget)t).TargetId == devtoolsTargetId).ConfigureAwait(false) as CdpTarget;

        if (target == null)
        {
            throw new PuppeteerException($"Missing target for DevTools page (id = {devtoolsTargetId})");
        }

        var initialized = await target.InitializedTask.ConfigureAwait(false) == InitializationStatus.Success;
        if (!initialized)
        {
            throw new PuppeteerException($"Failed to create target for DevTools page (id = {devtoolsTargetId})");
        }

        var page = await target.PageAsync().ConfigureAwait(false);
        if (page == null)
        {
            throw new PuppeteerException($"Failed to create a DevTools Page for target (id = {devtoolsTargetId})");
        }

        return page;
    }

    internal async Task<string> HasDevToolsTargetAsync(string pageTargetId)
    {
        var response = await Connection.SendAsync<TargetCreateTargetResponse>(
            "Target.getDevToolsTarget",
            new TargetActivateTargetRequest { TargetId = pageTargetId }).ConfigureAwait(false);
        return response.TargetId;
    }

    internal async Task DisposeContextAsync(string contextId)
    {
        await Connection.SendAsync("Target.disposeBrowserContext", new TargetDisposeBrowserContextRequest
        {
            BrowserContextId = contextId,
        }).ConfigureAwait(false);
        _contexts.TryRemove(contextId, out var _);
    }

    private static bool IsTargetExposed(CdpTarget target)
        => target.Type != TargetType.Tab && string.IsNullOrEmpty(target.TargetInfo.Subtype);

    private static bool IsDevToolsPageTarget(string url)
    {
        return url?.StartsWith("devtools://devtools/bundled/devtools_app.html", StringComparison.OrdinalIgnoreCase) == true;
    }

    // The version is not expected to change, so cache it and only call Browser.getVersion once.
    // This also avoids repeated calls when using Puppeteer with untrusted sessions.
    private Task<BrowserGetVersionResponse> GetVersionResponseAsync()
        => _versionTask ??= Connection.SendAsync<BrowserGetVersionResponse>("Browser.getVersion");

    private Task AttachAsync()
    {
        Connection.Disconnected += Connection_Disconnected;
        _subscriptions.Defer(() => Connection.Disconnected -= Connection_Disconnected);
        TargetManager.TargetAvailable += OnAttachedToTargetAsync;
        _subscriptions.Defer(() => TargetManager.TargetAvailable -= OnAttachedToTargetAsync);
        TargetManager.TargetGone += OnDetachedFromTargetAsync;
        _subscriptions.Defer(() => TargetManager.TargetGone -= OnDetachedFromTargetAsync);
        TargetManager.TargetChanged += OnTargetChanged;
        _subscriptions.Defer(() => TargetManager.TargetChanged -= OnTargetChanged);
        TargetManager.TargetDiscovered += TargetManager_TargetDiscovered;
        _subscriptions.Defer(() => TargetManager.TargetDiscovered -= TargetManager_TargetDiscovered);
        return TargetManager.InitializeAsync();
    }

    private void Detach()
    {
        _subscriptions.Dispose();
        TargetManager.Dispose();
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

        if (IsDevToolsPageTarget(targetInfo.Url))
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

                if (CloseCallback != null)
                {
                    await CloseCallback().ConfigureAwait(false);
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
        var target = (CdpTarget)e.Target;
        if (!IsTargetExposed(target))
        {
            return;
        }

        var args = new TargetChangedArgs(e.Target);
        OnTargetChanged(args);
        target.BrowserContext.OnTargetChanged(args);
    }

    private async void OnDetachedFromTargetAsync(object sender, TargetChangedArgs e)
    {
        try
        {
            var target = (CdpTarget)e.Target;
            target.InitializedTaskWrapper.TrySetResult(InitializationStatus.Aborted);
            target.CloseTaskWrapper.TrySetResult(true);

            if (!IsTargetExposed(target))
            {
                return;
            }

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
            if (!IsTargetExposed(target))
            {
                return;
            }

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
