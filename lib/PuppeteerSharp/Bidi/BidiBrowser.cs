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

    private BidiBrowser(Core.Browser browserCore, LaunchOptions options)
    {
        _options = options;
        BrowserCore = browserCore;
    }

    /// <inheritdoc />
    public override bool IsClosed { get; }

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

    internal Core.Browser BrowserCore { get; }

    internal override ProtocolType Protocol => ProtocolType.WebdriverBiDi;

    /// <inheritdoc />
    public override Task<string> GetVersionAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<string> GetUserAgentAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override void Disconnect() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task CloseAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IPage> NewPageAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override ITarget[] Targets() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public override IBrowserContext[] BrowserContexts() => throw new NotImplementedException();

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "We return the session, the browser needs to dispose the session")]
    internal static async Task<BidiBrowser> CreateAsync(
        BiDiDriver driver,
        LaunchOptions options,
        ILoggerFactory loggerFactory)
    {
        var session = await Session.FromAsync(
            driver,
            new NewCommandParameters
            {
                AlwaysMatch = new CapabilitiesRequest()
                {
                    AcceptInsecureCertificates = options.IgnoreHTTPSErrors,
                    AdditionalCapabilities = { ["webSocketUrl"] = true, },
                },
            },
            loggerFactory).ConfigureAwait(false);

        await session.SubscribeAsync(
            session.Info.Capabilities.BrowserName.ToLowerInvariant().Contains("firefox")
                ? SubscribeModules
                : [.. SubscribeModules, .. SubscribeCdpEvents]).ConfigureAwait(false);

        var browser = new BidiBrowser(session.Browser, options);
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

        return browserContext;
    }
}
