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

using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiBrowserContext : BrowserContext
{
    private readonly ConcurrentSet<BidiPage> _pages = [];

    private BidiBrowserContext(BidiBrowser browser, UserContext userContext, BidiBrowserContextOptions options)
    {
        Browser = browser;
    }

    internal TaskQueue ScreenshotTaskQueue => Browser.ScreenshotTaskQueue;

    /// <inheritdoc />
    public override Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task ClearPermissionOverridesAsync() => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task<IPage[]> PagesAsync() => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task<IPage> NewPageAsync() => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task CloseAsync() => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override ITarget[] Targets() => throw new System.NotImplementedException();

    internal static BidiBrowserContext From(
        BidiBrowser browser,
        UserContext userContext,
        BidiBrowserContextOptions options)
    {
        var context = new BidiBrowserContext(browser, userContext, options);
        context.Initialize();
        return context;
    }

    private void Initialize()
    {
        // Create targets for existing browsing contexts.
        foreach (var browsingContext in UserContext.BrowsingContexts)
        {
            CreatePage(browsingContext);
        }
    }

    private void CreatePage(BrowsingContext browsingContext)
    {
        var page = BidiPage.From(this, browsingContext);
        _pages.Add(page);
    }
}

