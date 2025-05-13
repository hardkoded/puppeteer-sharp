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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiBrowserContext : BrowserContext
{
    private readonly ConcurrentDictionary<BrowsingContext, BidiPage> _pages = [];
    private readonly ConcurrentDictionary<BidiPage, BidiPageTargetInfo> _targets = new();

    private BidiBrowserContext(BidiBrowser browser, UserContext userContext, BidiBrowserContextOptions options)
    {
        UserContext = userContext;
        Browser = browser;
        LoggerFactory = browser.LoggerFactory;
        DefaultViewport = options.DefaultViewport;
    }

    internal ILoggerFactory LoggerFactory { get; }

    internal ViewPortOptions DefaultViewport { get; set; }

    internal TaskQueue ScreenshotTaskQueue => Browser.ScreenshotTaskQueue;

    internal UserContext UserContext { get; }

    /// <inheritdoc />
    public override Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task ClearPermissionOverridesAsync() => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task<IPage[]> PagesAsync() => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override async Task<IPage> NewPageAsync()
    {
        var context = await UserContext.CreateBrowserContextAsync(WebDriverBiDi.BrowsingContext.CreateType.Tab).ConfigureAwait(false);

        if (!_pages.TryGetValue(context, out var page))
        {
            throw new PuppeteerException("Page is not found");
        }

        if (DefaultViewport != null)
        {
            try
            {
                await page.SetViewportAsync(DefaultViewport).ConfigureAwait(false);
            }
            catch
            {
                // No support for setViewport in Firefox.
            }
        }

        return page;
    }

    /// <inheritdoc />
    public override Task CloseAsync() => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override ITarget[] Targets()
        => _targets.SelectMany(target =>
            (ITarget[])[
                target.Value.BidiPageTarget,
                .. target.Value.FrameTargets.Values.ToArray(),
                .. target.Value.WorkerTargets.Values.ToArray(),
            ]).ToArray();

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

        UserContext.BrowsingContextCreated += (sender, args) =>
        {
            var browsingContext = args.BrowsingContext;
            var page = CreatePage(browsingContext);

            // We need to wait for the DOMContentLoaded as the
            // browsingContext still may be navigating from the about:blank
            browsingContext.DomContentLoaded += (o, eventArgs) =>
            {
                if (browsingContext.OriginalOpener == null)
                {
                    return;
                }

                foreach (var context in UserContext.BrowsingContexts)
                {
                    if (context.Id != browsingContext.OriginalOpener)
                    {
                        continue;
                    }

                    if (_pages.TryGetValue(context, out var originalOpenerPage))
                    {
                        originalOpenerPage.OnPopup(page);
                    }
                }
            };
        };
    }

    private BidiPage CreatePage(BrowsingContext browsingContext)
    {
        var page = BidiPage.From(this, browsingContext);
        _pages.AddOrUpdate(browsingContext, page, (_, _) => page);

        var pageTarget = new BidiPageTarget(page);
        var targetInfo = new BidiPageTargetInfo { BidiPageTarget = pageTarget };
        _targets.TryAdd(page, targetInfo);

        page.FrameAttached += (_, e) =>
        {
            var bidiFrame = (BidiFrame)e.Frame;
            var frameTarget = new BidiFrameTarget(bidiFrame);
            targetInfo.FrameTargets.TryAdd(bidiFrame, frameTarget);
        };

        page.FrameNavigated += (_, e) =>
        {
            var bidiFrame = (BidiFrame)e.Frame;
            OnTargetChanged(new TargetChangedArgs(
                targetInfo.FrameTargets.TryGetValue(bidiFrame, out var frameTarget)
                    ? frameTarget
                    : pageTarget));
        };

        page.FrameDetached += (_, e) =>
        {
            var bidiFrame = (BidiFrame)e.Frame;
            if (targetInfo.FrameTargets.TryRemove(bidiFrame, out var frameTarget))
            {
                OnTargetDestroyed(new TargetChangedArgs(frameTarget));
            }
        };

        page.WorkerCreated += (_, e) =>
        {
            var bidiWorker = (BidiWebWorker)e.Worker;
            var workerTarget = new BidiWorkerTarget(bidiWorker);
            targetInfo.WorkerTargets.TryAdd(bidiWorker, workerTarget);
        };

        page.WorkerDestroyed += (_, e) =>
        {
            var bidiWorker = (BidiWebWorker)e.Worker;
            if (targetInfo.WorkerTargets.TryRemove(bidiWorker, out var workerTarget))
            {
                OnTargetDestroyed(new TargetChangedArgs(workerTarget));
            }
        };

        page.Close += (_, _) =>
        {
            if (_targets.TryRemove(page, out _))
            {
                OnTargetDestroyed(new TargetChangedArgs(pageTarget));
            }
        };

        OnTargetCreated(new TargetChangedArgs(pageTarget));
        return page;
    }

    private class BidiPageTargetInfo
    {
        public BidiPageTarget BidiPageTarget { get; set; }

        public ConcurrentDictionary<BidiFrame, BidiFrameTarget> FrameTargets { get; set; } = new();

        public ConcurrentDictionary<BidiWebWorker, BidiWorkerTarget> WorkerTargets { get; set; } = new();
    }
}


