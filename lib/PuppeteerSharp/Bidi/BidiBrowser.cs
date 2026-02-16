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

#if !CDP_ONLY

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;
using WebDriverBiDi;
using WebDriverBiDi.Session;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// Represents a browser connected using the Bidi protocol.
/// </summary>
public class BidiBrowser : Browser
{
    /// <summary>
    /// Time in milliseconds for process to exit gracefully.
    /// </summary>
    private const int CloseTimeout = 5000;

    private readonly IBrowserOptions _options;
    private readonly ConcurrentDictionary<UserContext, BidiBrowserContext> _browserContexts = new();
    private readonly ILogger<BidiBrowser> _logger;
    private readonly BidiBrowserTarget _target;
    private readonly string _webSocketEndpoint;
    private bool _isClosed;

    private BidiBrowser(Core.Browser browserCore, IBrowserOptions options, ILoggerFactory loggerFactory, string webSocketEndpoint)
    {
        _target = new BidiBrowserTarget(this);
        _options = options;
        BrowserCore = browserCore;
        _webSocketEndpoint = webSocketEndpoint;
        _logger = loggerFactory.CreateLogger<BidiBrowser>();
        LoggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public override bool IsClosed => _isClosed;

    /// <inheritdoc />
    public override bool IsConnected => !BrowserCore.IsDisconnected;

    /// <inheritdoc />
    public override string WebSocketEndpoint => _webSocketEndpoint ?? (Launcher?.EndPoint != null ? Launcher.EndPoint + "/session" : null);

    /// <inheritdoc />
    public override ITarget Target => _target;

    /// <inheritdoc/>
    public override IBrowserContext DefaultContext => _browserContexts.TryGetValue(BrowserCore.DefaultUserContext, out var context) ? context : null;

    internal static string[] SubscribeModules { get; } =
    [
        "browsingContext",
        "network",
        "log",
        "script",
        "input",
    ];

    internal static string[] SubscribeCdpEvents { get; } =
    [
        "cdp.Debugger.scriptParsed",
        "cdp.CSS.styleSheetAdded",
        "cdp.Runtime.executionContextsCleared",
        "cdp.Tracing.tracingComplete",
        "cdp.Network.requestWillBeSent",
        "cdp.Debugger.scriptParsed",
        "cdp.Page.screencastFrame",
    ];

    internal ILoggerFactory LoggerFactory { get; set; }

    internal BiDiDriver Driver => BrowserCore.Session.Driver;

    internal Core.Browser BrowserCore { get; }

    internal override ProtocolType Protocol => ProtocolType.WebdriverBiDi;

    // TODO: Implement
    internal bool CdpSupported => false;

    internal string BrowserVersion => BrowserCore.Session.Info.Capabilities.BrowserVersion;

    internal string BrowserName => BrowserCore.Session.Info.Capabilities.BrowserName;

    /// <inheritdoc />
    public override Task<string> GetVersionAsync() => Task.FromResult($"{BrowserName}/{BrowserVersion}");

    /// <inheritdoc />
    public override Task<string> GetUserAgentAsync() => Task.FromResult(BrowserCore.Session.Info.Capabilities.UserAgent);

    /// <inheritdoc />
    public override void Disconnect()
    {
        Driver.StopAsync().GetAwaiter().GetResult();
        Detach();
    }

    /// <inheritdoc />
    public override async Task CloseAsync()
    {
        if (_isClosed)
        {
            return;
        }

        _isClosed = true;
        try
        {
            try
            {
                await BrowserCore.CloseAsync().ConfigureAwait(false);

                if (Launcher != null)
                {
                    // Notify process that exit is expected, but should be enforced if it
                    // doesn't occur within the close timeout.
                    var closeTimeout = TimeSpan.FromMilliseconds(CloseTimeout);
                    await Launcher.EnsureExitAsync(closeTimeout).ConfigureAwait(false);
                }
            }
            finally
            {
                try
                {
                    await Driver.StopAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to stop driver");
                }

                Detach();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to close browser");

            if (Launcher != null)
            {
                await Launcher.KillAsync().ConfigureAwait(false);
            }
        }

        OnClosed();
    }

    /// <inheritdoc />
    public override Task<IPage> NewPageAsync(CreatePageOptions options = null) => DefaultContext.NewPageAsync(options);

    /// <inheritdoc/>
    public override Task<ScreenInfo[]> ScreensAsync()
        => throw new NotSupportedException("Screens is not supported in WebDriver BiDi.");

    /// <inheritdoc/>
    public override Task<ScreenInfo> AddScreenAsync(AddScreenParams @params)
        => throw new NotSupportedException("AddScreen is not supported in WebDriver BiDi.");

    /// <inheritdoc/>
    public override Task RemoveScreenAsync(string screenId)
        => throw new NotSupportedException("RemoveScreen is not supported in WebDriver BiDi.");

    /// <inheritdoc />
    public override ITarget[] Targets()
        =>
        [
            _target,
            .. BrowserContexts().SelectMany(context => ((BidiBrowserContext)context).Targets()).ToArray()
        ];

    /// <inheritdoc />
    public override async Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null)
    {
        var userContext = await BrowserCore.CreateUserContextAsync().ConfigureAwait(false);
        return CreateBrowserContext(userContext);
    }

    /// <inheritdoc />
    public override IBrowserContext[] BrowserContexts() =>
        BrowserCore.UserContexts
            .Select(userContext => _browserContexts.TryGetValue(userContext, out var context) ? context : null)
            .Where(context => context != null)
            .Cast<IBrowserContext>()
            .ToArray();

    /// <inheritdoc />
    public override async Task<WindowBounds> GetWindowBoundsAsync(string windowId)
    {
        var result = await Driver.Browser.GetClientWindowsAsync().ConfigureAwait(false);
        var window = result.ClientWindows.FirstOrDefault(w => w.ClientWindowId == windowId)
            ?? throw new PuppeteerException("Window not found");

        return new WindowBounds
        {
            Left = (int)window.X,
            Top = (int)window.Y,
            Width = (int)window.Width,
            Height = (int)window.Height,
            WindowState = MapFromClientWindowState(window.State),
        };
    }

    /// <inheritdoc />
    public override async Task SetWindowBoundsAsync(string windowId, WindowBounds windowBounds)
    {
        if (windowBounds == null)
        {
            throw new ArgumentNullException(nameof(windowBounds));
        }

        var windowState = windowBounds.WindowState ?? WindowState.Normal;
        var parameters = new WebDriverBiDi.Browser.SetClientWindowStateCommandParameters(windowId)
        {
            State = MapToClientWindowState(windowState),
        };

        if (windowState == WindowState.Normal)
        {
            if (windowBounds.Left.HasValue)
            {
                parameters.X = (ulong)windowBounds.Left.Value;
            }

            if (windowBounds.Top.HasValue)
            {
                parameters.Y = (ulong)windowBounds.Top.Value;
            }

            if (windowBounds.Width.HasValue)
            {
                parameters.Width = (ulong)windowBounds.Width.Value;
            }

            if (windowBounds.Height.HasValue)
            {
                parameters.Height = (ulong)windowBounds.Height.Value;
            }
        }

        await Driver.Browser.SetClientWindowStateAsync(parameters).ConfigureAwait(false);
    }

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "We return the session, the browser needs to dispose the session")]
    internal static Task<BidiBrowser> CreateAsync(
        BiDiDriver driver,
        LaunchOptions options,
        ILoggerFactory loggerFactory,
        LauncherBase launcher)
        => CreateAsync(driver, options, loggerFactory, launcher, null);

    internal static Task<BidiBrowser> CreateAsync(
        BiDiDriver driver,
        ConnectOptions options,
        ILoggerFactory loggerFactory,
        LauncherBase launcher)
        => CreateAsync(driver, options, loggerFactory, launcher, null);

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "We return the session, the browser needs to dispose the session")]
    internal static async Task<BidiBrowser> CreateAsync(
        BiDiDriver driver,
        IBrowserOptions options,
        ILoggerFactory loggerFactory,
        LauncherBase launcher,
        string webSocketEndpoint)
    {
        var session = await Session.FromAsync(
            driver,
            new NewCommandParameters
            {
                Capabilities = new CapabilitiesRequest()
                {
                    AlwaysMatch = new CapabilityRequest()
                    {
                        AcceptInsecureCertificates = options.AcceptInsecureCerts,
                        AdditionalCapabilities = { ["webSocketUrl"] = true, },

                        // Tell the browser not to auto-handle prompts so we can handle them via the Dialog API.
                        UnhandledPromptBehavior = new UserPromptHandler
                        {
                            Default = UserPromptHandlerType.Ignore,
                        },
                    },
                },
            },
            loggerFactory).ConfigureAwait(false);

        await session.SubscribeAsync(
            session.Info.Capabilities.BrowserName.ToLowerInvariant().Contains("firefox")
                ? SubscribeModules
                : [.. SubscribeModules, .. SubscribeCdpEvents]).ConfigureAwait(false);

        // Add data collectors for request and response bodies.
        // This enables network.getData to return response/request bodies.
        // Data collectors might not be implemented for specific data types, so create them
        // separately and ignore protocol errors.
        foreach (var dataType in new[] { WebDriverBiDi.Network.DataType.Request, WebDriverBiDi.Network.DataType.Response })
        {
            try
            {
                // Buffer size of 20 MB is equivalent to CDP
                await driver.Network.AddDataCollectorAsync(
                    new WebDriverBiDi.Network.AddDataCollectorCommandParameters(20_000_000, dataType)).ConfigureAwait(false);
            }
            catch (WebDriverBiDi.WebDriverBiDiException)
            {
                // Ignore errors - data collectors might not be supported for all data types
            }
        }

        var browser = new BidiBrowser(session.Browser, options, loggerFactory, webSocketEndpoint) { Launcher = launcher };
        browser.InitializeAsync();
        return browser;
    }

    private static WindowState MapFromClientWindowState(WebDriverBiDi.Browser.ClientWindowState state) => state switch
    {
        WebDriverBiDi.Browser.ClientWindowState.Normal => WindowState.Normal,
        WebDriverBiDi.Browser.ClientWindowState.Minimized => WindowState.Minimized,
        WebDriverBiDi.Browser.ClientWindowState.Maximized => WindowState.Maximized,
        WebDriverBiDi.Browser.ClientWindowState.Fullscreen => WindowState.Fullscreen,
        _ => WindowState.Normal,
    };

    private static WebDriverBiDi.Browser.ClientWindowState MapToClientWindowState(WindowState state) => state switch
    {
        WindowState.Normal => WebDriverBiDi.Browser.ClientWindowState.Normal,
        WindowState.Minimized => WebDriverBiDi.Browser.ClientWindowState.Minimized,
        WindowState.Maximized => WebDriverBiDi.Browser.ClientWindowState.Maximized,
        WindowState.Fullscreen => WebDriverBiDi.Browser.ClientWindowState.Fullscreen,
        _ => WebDriverBiDi.Browser.ClientWindowState.Normal,
    };

    private void InitializeAsync()
    {
        // Initializing existing contexts.
        foreach (var userContext in BrowserCore.UserContexts)
        {
            CreateBrowserContext(userContext);
        }

        // Subscribe to browserCore disconnected event
        BrowserCore.Disconnected += OnBrowserCoreDisconnected;

        // Subscribe to process exit event if we have a launcher
        if (Launcher?.Process != null)
        {
            Launcher.Process.Exited += OnProcessExited;
        }
    }

    private void OnBrowserCoreDisconnected(object sender, ClosedEventArgs e)
    {
        _isClosed = true;
        OnDisconnected();
    }

    private void OnProcessExited(object sender, EventArgs e)
    {
        BrowserCore.Dispose();
    }

    private void Detach()
    {
        // Unsubscribe from browser core events
        BrowserCore.Disconnected -= OnBrowserCoreDisconnected;

        // Unsubscribe from process exit event
        if (Launcher?.Process != null)
        {
            Launcher.Process.Exited -= OnProcessExited;
        }

        foreach (var context in _browserContexts.Values)
        {
            context.TargetCreated -= (sender, args) => OnTargetCreated(args);
            context.TargetChanged -= (sender, args) => OnTargetChanged(args);
            context.TargetDestroyed -= (sender, args) => OnTargetDestroyed(args);
        }
    }

    private BidiBrowserContext CreateBrowserContext(UserContext userContext)
    {
        var browserContext = BidiBrowserContext.From(
            this,
            userContext,
            new BidiBrowserContextOptions() { DefaultViewport = _options.DefaultViewport, });

        _browserContexts.TryAdd(userContext, browserContext);

        browserContext.TargetCreated += (sender, args) => OnTargetCreated(args);
        browserContext.TargetChanged += (sender, args) => OnTargetChanged(args);
        browserContext.TargetDestroyed += (sender, args) => OnTargetDestroyed(args);

        return browserContext;
    }
}

#endif
