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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;
using WebDriverBiDi.Permissions;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiBrowserContext : BrowserContext
{
    private readonly ConcurrentDictionary<BrowsingContext, BidiPage> _pages = [];
    private readonly ConcurrentDictionary<BidiPage, BidiPageTargetInfo> _targets = new();
    private readonly List<(string Origin, OverridePermission Permission)> _overrides = [];
    private readonly ILogger<BidiBrowserContext> _logger;

    private BidiBrowserContext(BidiBrowser browser, UserContext userContext, BidiBrowserContextOptions options)
    {
        UserContext = userContext;
        Browser = browser;
        DefaultViewport = options.DefaultViewport;
        _logger = browser.LoggerFactory?.CreateLogger<BidiBrowserContext>();
    }

    internal ViewPortOptions DefaultViewport { get; set; }

    internal TaskQueue ScreenshotTaskQueue => Browser.ScreenshotTaskQueue;

    internal UserContext UserContext { get; }

    /// <inheritdoc />
    public override async Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions)
    {
        var permissionsSet = new HashSet<OverridePermission>(permissions);

        // We need to set all permissions - grant the ones in the list, deny the rest
        var tasks = new List<Task>();
        foreach (OverridePermission permission in Enum.GetValues(typeof(OverridePermission)))
        {
            var state = permissionsSet.Contains(permission)
                ? PermissionState.Granted
                : PermissionState.Denied;

            var permissionName = GetPermissionName(permission);

            var task = UserContext.SetPermissionsAsync(origin, permissionName, state);
            _overrides.Add((origin, permission));

            // Denying some outdated permissions might fail, so we catch those errors
            if (!permissionsSet.Contains(permission))
            {
                task = task.ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            // Log the error but don't throw
                            ((BidiBrowser)Browser).LoggerFactory?.CreateLogger<BidiBrowserContext>()
                                .LogDebug(t.Exception, "Failed to deny permission {Permission}", permission);
                        }
                    },
                    TaskScheduler.Default);
            }

            tasks.Add(task);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task ClearPermissionOverridesAsync()
    {
        var tasks = new List<Task>();
        foreach (var (origin, permission) in _overrides.ToArray())
        {
            var permissionName = GetPermissionName(permission);
            tasks.Add(UserContext.SetPermissionsAsync(origin, permissionName, PermissionState.Prompt)
                .ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            // Log the error but don't throw
                            ((BidiBrowser)Browser).LoggerFactory?.CreateLogger<BidiBrowserContext>()
                                .LogDebug(t.Exception, "Failed to reset permission {Permission}", permission);
                        }
                    },
                    TaskScheduler.Default));
        }

        _overrides.Clear();
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task<IPage[]> PagesAsync() => Task.FromResult(_pages.Values.Cast<IPage>().ToArray());

    /// <inheritdoc />
    public override async Task<IPage> NewPageAsync(CreatePageOptions options = null)
    {
        var type = options?.Type == CreatePageType.Window
            ? WebDriverBiDi.BrowsingContext.CreateType.Window
            : WebDriverBiDi.BrowsingContext.CreateType.Tab;

        var context = await UserContext.CreateBrowserContextAsync(
            type,
            background: options?.Background).ConfigureAwait(false);

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
            catch (Exception ex)
            {
                // Tolerate not supporting browsingContext.setViewport. Only log it.
                _logger?.LogDebug(ex, "Failed to set viewport");
            }
        }

        if (options?.Type == CreatePageType.Window && options?.WindowBounds != null)
        {
            try
            {
                var windowId = await page.WindowIdAsync().ConfigureAwait(false);
                await Browser.SetWindowBoundsAsync(windowId, options.WindowBounds).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Tolerate not supporting browser.setClientWindowState. Only log it.
                _logger?.LogDebug(ex, "Failed to set window bounds");
            }
        }

        return page;
    }

    /// <inheritdoc />
    public override async Task CloseAsync()
    {
        if (UserContext.Id == UserContext.DEFAULT)
        {
            throw new PuppeteerException("Default BrowserContext cannot be closed!");
        }

        try
        {
            await UserContext.RemoveAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ((BidiBrowser)Browser).LoggerFactory?.CreateLogger<BidiBrowserContext>()
                .LogDebug(ex, "Failed to close browser context");
        }

        _targets.Clear();
    }

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
        var context = new BidiBrowserContext(browser, userContext, options)
        {
            Id = userContext.Id,
        };
        context.Initialize();
        return context;
    }

    private static string GetPermissionName(OverridePermission permission)
    {
        return permission switch
        {
            OverridePermission.Geolocation => "geolocation",
            OverridePermission.Midi => "midi",
            OverridePermission.Notifications => "notifications",
            OverridePermission.Camera => "camera",
            OverridePermission.Microphone => "microphone",
            OverridePermission.BackgroundSync => "background-sync",
            OverridePermission.Sensors => "accelerometer",
            OverridePermission.AccessibilityEvents => "accessibility-events",
            OverridePermission.ClipboardReadWrite => "clipboard-read",
            OverridePermission.PaymentHandler => "payment-handler",
            OverridePermission.MidiSysex => "midi-sysex",
            OverridePermission.IdleDetection => "idle-detection",
            OverridePermission.PersistentStorage => "persistent-storage",
            OverridePermission.LocalNetworkAccess => "local-network-access",
            _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, "Unknown permission"),
        };
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
            _pages.TryRemove(page.BidiMainFrame.BrowsingContext, out _);
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

#endif
