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
    private readonly LaunchOptions _options;
    private readonly ConcurrentSet<BidiBrowserContext> _browserContexts = [];
    private readonly ILogger<BidiBrowser> _logger;
    private readonly BidiBrowserTarget _target;

    private BidiBrowser(Core.Browser browserCore, LaunchOptions options, ILoggerFactory loggerFactory)
    {
        _target = new BidiBrowserTarget(this);
        _options = options;
        BrowserCore = browserCore;
        _logger = loggerFactory.CreateLogger<BidiBrowser>();
    }

    /// <inheritdoc />
    public override bool IsClosed { get; }

    /// <inheritdoc />
    public override ITarget Target => _target;

    internal static string[] SubscribeModules { get; } =
    [
        "browsingContext",
        "network",
        "log",
        "script",
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
    public override void Disconnect() => throw new NotImplementedException();

    /// <inheritdoc />
    public override async Task CloseAsync()
    {
        try
        {
            await BrowserCore.CloseAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to close connection");
        }
        finally
        {
            await Driver.StopAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override Task<IPage> NewPageAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override ITarget[] Targets()
        =>
        [
            _target,
            .. _browserContexts.SelectMany(context => context.Targets()).ToArray()
        ];

    /// <inheritdoc />
    public override async Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null)
    {
        var userContext = await BrowserCore.CreateUserContextAsync().ConfigureAwait(false);
        return CreateBrowserContext(userContext);
    }

    /// <inheritdoc />
    public override IBrowserContext[] BrowserContexts() => throw new NotImplementedException();

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "We return the session, the browser needs to dispose the session")]
    internal static async Task<BidiBrowser> CreateAsync(
        BiDiDriver driver,
        LaunchOptions options,
        ILoggerFactory loggerFactory,
        LauncherBase launcher)
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
                    },
                },
            },
            loggerFactory).ConfigureAwait(false);

        await session.SubscribeAsync(
            session.Info.Capabilities.BrowserName.ToLowerInvariant().Contains("firefox")
                ? SubscribeModules
                : [.. SubscribeModules, .. SubscribeCdpEvents]).ConfigureAwait(false);

        var browser = new BidiBrowser(session.Browser, options, loggerFactory) { Launcher = launcher };
        browser.InitializeAsync();
        return browser;
    }

    private void InitializeAsync()
    {
        // Initializing existing contexts.
        foreach (var userContext in BrowserCore.UserContexts)
        {
            CreateBrowserContext(userContext);
        }
    }

    private BidiBrowserContext CreateBrowserContext(UserContext userContext)
    {
        var browserContext = BidiBrowserContext.From(
            this,
            userContext,
            new BidiBrowserContextOptions() { DefaultViewport = _options.DefaultViewport, });

        _browserContexts.Add(browserContext);

        browserContext.TargetCreated += (sender, args) => OnTargetCreated(args);
        browserContext.TargetDestroyed += (sender, args) => OnTargetDestroyed(args);

        return browserContext;
    }
}

