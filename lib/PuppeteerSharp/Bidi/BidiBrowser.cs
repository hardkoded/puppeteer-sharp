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
using System.Diagnostics;
using System.Threading.Tasks;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// Represents a browser connected using the Bidi protocol.
/// </summary>
public class BidiBrowser : Browser
{
    /// <inheritdoc />
    public override bool IsClosed { get; }

    internal override ProtocolType Protocol => ProtocolType.WebdriverBiDi;

    /// <inheritdoc />
    public override Task<string> GetVersionAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<string> GetUserAgentAsync() => throw new NotImplementedException();

    internal static async Task<BidiBrowser> CreateAsync(
        int timeout,
        int protocolTimeout,
        bool slowMo,
        ViewPortOptions defaultViewPort,
        bool ignoreHTTPSErrors)
    {
        var browser = new BidiBrowser(
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

    /// <inheritdoc />
    public override void Disconnect() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task CloseAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IPage> NewPageAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override ITarget[] Targets() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override IBrowserContext[] BrowserContexts() => throw new NotImplementedException();
}
